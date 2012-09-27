<%@ Page Title="" Language="C#" MasterPageFile="~/Site2.Master" AutoEventWireup="true" CodeBehind="PayForGoods.aspx.cs" Inherits="SuperMarketWeb.PayForGoods" %>
<%@ Import Namespace="SuperMarketModel"  %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
���ã���ӭ����������̨����ѡ��Ϊ�����������Ա��
<asp:GridView ID="GridView1" runat="server" BackColor="#DEBA84" 
        BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="3" 
        CellSpacing="2" onselectedindexchanged="GridView1_SelectedIndexChanged" 
        Width="479px" AutoGenerateColumns="False">
        <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
        <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
        <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
        <Columns>
            <asp:ButtonField CommandName="Select" HeaderText="ѡ��" ShowHeader="True" 
                Text="ѡ��" />
            <asp:TemplateField HeaderText="����Ա">
                <ItemTemplate>
                    <%# ((SuperMarketBLL.CashierRegisterBIZ)Container.DataItem).CurrCashier.CashierName  %>
                </ItemTemplate>
               
                <ItemStyle Width="100px" />
               
            </asp:TemplateField>
            <asp:TemplateField HeaderText="����">
                <ItemTemplate>
                    <%# ((SuperMarketBLL.CashierRegisterBIZ)Container.DataItem).CurrCashier.WorkNumber  %>
                </ItemTemplate>
                <ItemStyle Width="100px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="��������">
                <ItemTemplate>
                     <%# ((SuperMarketBLL.CashierRegisterBIZ)Container.DataItem).CurrCRManchines.CashRegisterNo   %>
                </ItemTemplate>
                <ItemStyle Width="200px" />
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <p>
        <asp:Button ID="btnWaite" runat="server" Text="�ڴ�����̨�Ŷ�" 
            onclick="btnWaite_Click" OnClientClick ="" />
        <asp:Label ID="lblQueue" runat="server"></asp:Label>
    </p>
    <p>
        �����εĹ���۸��嵥��
    <asp:GridView ID="gvSPCart" runat="server" BackColor="#DEBA84" 
        BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="3" 
        CellSpacing="2"  
            AutoGenerateColumns="False" 
       >
        <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
        <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
        <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
        <Columns>
            <asp:ButtonField CommandName="Select" HeaderText="ѡ��" ShowHeader="True" 
                Text="ѡ��" />
            <asp:BoundField DataField="SerialNumber" HeaderText="�����" ReadOnly="True" />
            <asp:BoundField DataField="GoodsName" HeaderText="��Ʒ����" ReadOnly="True" />
            <asp:BoundField DataField="GoodsPrice" HeaderText="����" ReadOnly="True" />
            <asp:BoundField DataField="DiscountPrice" HeaderText="�ۿۼ�" ReadOnly="True" />
            <asp:BoundField DataField="GoodsNumber" HeaderText="��������" />
            <asp:BoundField DataField="GoodsMoney" HeaderText="ʵ�ս��" />
            <asp:CommandField ShowDeleteButton="True" />
        </Columns>
    </asp:GridView>  
    </p>
    <p>
        ˵��������Ĺ��������ı�Ϊ0����ʾʵ�ʿ��Ϊ0���������ۡ�</p>
    <p>
    ���ϼơ�<asp:Label ID ="lblAmout" runat="server"></asp:Label> Ԫ 
    </p>
    <p>
    &nbsp;<asp:Button ID="btnOK" runat="server" onclick="btnOK_Click" Text="ͬ��֧��" />
&nbsp;
        <asp:Button ID="btnCancel" runat="server" Text="���ؼ�������" 
            onclick="btnCancel_Click" />
    &nbsp;
        <asp:Button ID="btnQuitBuy" runat="server" onclick="btnQuitBuy_Click" 
            Text="�������ι���" />
    </p>

</asp:Content>
