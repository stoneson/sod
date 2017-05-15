﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessagePublishService;
using System.Threading;

namespace MessagePublisher
{
    /// <summary>
    /// 消息中心；
    /// </summary>
    public class MessageCenter
    {
        #region MessageCenter 的单例实现
        private static readonly object _syncLock = new object();//线程同步锁；
        private static MessageCenter _instance;
        /// <summary>
        /// 返回 MessageCenter 的唯一实例；
        /// </summary>
        public static MessageCenter Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new MessageCenter();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 保证单例的私有构造函数；
        /// </summary>
        private MessageCenter() { }

        #endregion

        public event EventHandler<MessageListenerEventArgs> ListenerAdded;

        public event EventHandler<MessageListenerEventArgs> ListenerRemoved;

        public event EventHandler<MessageNotifyErrorEventArgs> NotifyError;

        public event EventHandler<MessageListenerEventArgs> ListenerAcceptMessage;

        public event EventHandler<MessageListenerEventArgs> ListenerEventMessage;

        public event EventHandler<MessageRequestEventArgs> ListenerRequestMessage;

        private List<MessageListener> _listeners = new List<MessageListener>(0);

        /// <summary>
        /// 用户凭据字典，Ｋｅｙ＝凭据，Ｖａｌｕｅ＝用户消息对象
        /// </summary>
        private Dictionary<string, MessageUser> _userIdentity = new Dictionary<string, MessageUser>();

        /// <summary>
        /// 获取当前监听器数量
        /// </summary>
        public int ListenerCount
        {
            get { return _listeners.Count; }
        }

        /// <summary>
        /// 检查当前监听器，返回有效地数量(注：监听器可能存在没有正确移除的情况，比如客户端非正常断开，需要在此处离)
        /// </summary>
        /// <returns></returns>
        public int[] CheckListeners()
        {
            int count = 0;
            int activeCount = 0;
            MessageListener[] listeners = this.GetListeners();
            if (listeners != null && listeners.Length > 0)
            {
                foreach (var item in listeners)
                {
                    //检查连接是否还有效
                    if (this.NotifyOneMessage(item, 0, "heart beat"))
                    {
                        count++;
                        if (DateTime.Now.Subtract(item.AtTime).TotalSeconds > 30)
                            activeCount++;
                    }
                }
            }
            return new int[] { count, activeCount };
        }

        public MessageListener GetListener(string ip, int port)
        {
            MessageListener lsn = null;
            lock (_syncLock)
            {
                lsn = _listeners.FirstOrDefault(p => p.FromIP == ip && p.FromPort == port);
            }

            //由主程序定期执行 CheckListeners 方法，故此注释下面的代码 dth,2012.5.3
            //if (lsn != null && DateTime.Now.Second % 12 == 0)
            //{
            //    //检查连接是否还有效
            //    if (!this.NotifyOneMessage(lsn, 0, "heart beat"))
            //        lsn = null;
            //}
            return lsn;
        }

        public MessageListener GetListener(MessageListener newListener)
        {
            lock (_syncLock)
            {
                MessageListener lsn = _listeners.FirstOrDefault(p => p.FromIP == newListener.FromIP && p.FromPort == newListener.FromPort);
                return lsn ?? newListener;
            }
        }

        public MessageListener[] GetListeners()
        {
            return _listeners.ToArray();
        }

        public void AddListener(MessageListener listener)
        {
            AddListener(listener, "");
        }

        public void AddListener(MessageListener listener, string identity)
        {
            lock (_syncLock)
            {
                if (_listeners.Contains(listener))
                {
                    throw new InvalidOperationException("重复注册相同的监听器！");
                }
                //_listeners.Add(listener); 

                //立即验证凭据
                if (string.IsNullOrEmpty(identity))
                    identity = listener.GetIdentity();

                string key = listener.FromIP + ":" + listener.FromPort + ":" + identity;
                if (_userIdentity.ContainsKey(key))
                    throw new InvalidOperationException("重复注册相同的客户端标识：" + key);

                MessageUser user = MessageUser.GetUserFromMessageString(identity);
                if (user == null)
                {
                    listener.Response(0, "订阅失败：不合法的客户端凭据！");
                    //OnNotifyError(listener, new Exception("不合法的客户端凭据！"));
                    //RemoveListener(listener);
                    OnEventMessage(listener, "注册监听器失败：不合法的客户端凭据！");
                    listener.Close(0);
                    return;
                }
                ValidateUser(user);
                if (!user.Validated)
                {
                    listener.Response(0, "订阅失败：服务器用户验证未通过！");
                    //OnNotifyError(listener, new Exception("用户验证未通过！"));
                    //RemoveListener(listener);
                    OnEventMessage(listener, "注册监听器失败：用户验证未通过！");
                    listener.Close(0);
                    return;
                }
                listener.Identity = user.HID;
                _userIdentity.Add(key, user);
                _listeners.Add(listener);
                listener.SessionID = key + ":" + DateTime.Now.ToString("HHmmssfff");
            }

            if (this.ListenerAdded != null)
            {
                this.ListenerAdded(this, new MessageListenerEventArgs(listener));
            }
        }

        public void RemoveListener(MessageListener listener)
        {
            lock (_syncLock)
            {
                if (this._listeners.Remove(listener))
                {
                    string keyFlag = listener.FromIP + ":" + listener.FromPort;
                    foreach (string key in _userIdentity.Keys)
                    {
                        if (key.Contains(keyFlag))
                        {
                            _userIdentity.Remove(key);
                            break;
                        }
                    }
                    listener.Removed = true;
                }
                else
                {
                    //throw new InvalidOperationException("要移除的监听器不存在！");
                    OnEventMessage(listener, "要移除的监听器不存在！");
                }
            }
            if (this.ListenerRemoved != null)
            {
                this.ListenerRemoved(this, new MessageListenerEventArgs(listener));
            }
        }

        /// <summary>
        /// 向所有注册为String类型的监听器发布消息
        /// </summary>
        /// <param name="message"></param>
        public int NotifyMessage(string message)
        {
            MessageListener[] listeners = _listeners.Where(p => p.RequestMessageType() == "System.String").ToArray();
            if (listeners.Length > 0)
                NotifyMessage(message, listeners);
            return listeners.Length;
        }

        /// <summary>
        /// 向指定的监听器集合发布消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="listeners"></param>
        public void NotifyMessage(string message, MessageListener[] listeners)
        {
            foreach (MessageListener lstn in listeners)
            {
                NotifyOneMessage(lstn, 1, message);//有问题
            }
        }

        /// <summary>
        /// 以安全的方式，给一个监听器发送消息
        /// </summary>
        /// <param name="lstn"></param>
        /// <param name="message"></param>
        public bool NotifyOneMessage(MessageListener lstn, int id, string message)
        {
            try
            {
                lstn.Notify(id, message);
                return true;
            }
            catch (Exception ex)
            {
                OnNotifyError(lstn, ex);
            }
            return false;
        }

        public bool ResponseMessage(MessageListener lstn, int id, string message)
        {
            try
            {
                lstn.Response(id, message);
                return true;
            }
            catch (Exception ex)
            {
                OnNotifyError(lstn, ex);
            }
            return false;
        }

        public void AcceptMessage(MessageListener listener)
        {
            if (_listeners.Contains(listener))
            {
                if (ListenerAcceptMessage != null)
                {
                    //由于此方法相关的服务接口特性为 IsOneWay = true ,listener 将很快过期
                    MessageListener currLsn = this.GetListener(listener.FromIP, listener.FromPort);
                    if (currLsn != null)
                    {
                        currLsn.FromMessage = listener.FromMessage;
                        currLsn.MessageID = listener.MessageID;
                        //string showMsg = currLsn.FromMessage.Length > 1000 ? currLsn.FromMessage.Substring(0, 1000) : currLsn.FromMessage;
                        //Console.WriteLine("MessageListener FromMessage:{0},MessageID:{1}", showMsg, currLsn.MessageID);
                        this.ListenerAcceptMessage(this, new MessageListenerEventArgs(currLsn));
                    }
                }
            }
            else
            {
                OnNotifyError(listener, new Exception("监听器未被注册。"));
            }
        }

        public string RequestMessage(MessageListener listener)
        {
            if (_listeners.Contains(listener))
            {
                if (ListenerRequestMessage != null)
                {
                    //依赖于外部对消息进行处理，取得ResultText
                    MessageListener currLsn = this.GetListener(listener);
                    currLsn.FromMessage = listener.FromMessage;
                    MessageRequestEventArgs args = new MessageRequestEventArgs(currLsn);
                    ListenerRequestMessage(this, args);
                    return args.ResultText;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                OnNotifyError(listener, new Exception("监听器未被注册。"));
            }
            return "";
        }

        private void OnNotifyError(MessageListener listener, Exception error)
        {
            if (this.NotifyError == null || listener == null)
            {
                return;
            }
            MessageNotifyErrorEventArgs args = new MessageNotifyErrorEventArgs(listener, error);
            ThreadPool.QueueUserWorkItem(delegate(object state)
            {
                this.NotifyError(this, state as MessageNotifyErrorEventArgs);
            }, args);
        }

        private void OnEventMessage(MessageListener listener, string messageText)
        {
            if (ListenerEventMessage != null)
                ListenerEventMessage(this, new MessageListenerEventArgs(listener, messageText));
        }

        /// <summary>
        /// 验证消息的用户是否合法
        /// </summary>
        /// <param name="user"></param>
        private void ValidateUser(MessageUser user)
        {
            if (user.Name == "PDF.NET" && user.Password == "20111230")
            {
                user.Validated = true;
            }
            else
            {
                user.Validated = false;
            }
        }

        /// <summary>
        /// 通道关闭事件处理程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Channel_Closing(object sender, EventArgs e)
        {
            lock (_syncLock)
            {
                IMessageListenerCallBack clientCallback = (IMessageListenerCallBack)sender;
                var messageListener = _listeners.Find(p => p.GetListener() == clientCallback);
                if (messageListener != null)
                {
                    RemoveListener(messageListener);
                    Console.WriteLine("Channel_Closing");
                }
            }
        }
    }
}
