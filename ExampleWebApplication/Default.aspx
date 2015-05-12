<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Erwine.Leonard.T.ExampleWebApplication.DefaultPage" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <table>
        <tr>
            <td style="vertical-align: top; text-align: right">1</td>
            <td>
                <asp:Literal ID="Literal1" runat="server" Mode="PassThrough" OnLoad="Literal1_Load"></asp:Literal></td>
        </tr>
        <tr>
            <td style="vertical-align: top; text-align: right">2</td>
            <td>
                <asp:Literal ID="Literal2" runat="server" Mode="PassThrough" OnLoad="Literal2_Load"></asp:Literal></td>
        </tr>
        <tr>
            <td style="vertical-align: top; text-align: right">3</td>
            <td>
                <asp:Literal ID="Literal3" runat="server" Mode="PassThrough" OnLoad="Literal3_Load"></asp:Literal></td>
        </tr>
        <tr>
            <td style="vertical-align: top; text-align: right">4</td>
            <td>
                <asp:Literal ID="Literal4" runat="server" Mode="PassThrough" OnLoad="Literal4_Load"></asp:Literal></td>
        </tr>
        <tr>
            <td style="vertical-align: top; text-align: right">5</td>
            <td>
                <asp:Literal ID="Literal5" runat="server" Mode="PassThrough" OnLoad="Literal5_Load"></asp:Literal></td>
        </tr>
    </table>
</asp:Content>
