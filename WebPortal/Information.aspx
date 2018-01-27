<%@ Page Title="" Language="C#" MasterPageFile="~/Homepage.master" AutoEventWireup="true" CodeFile="Information.aspx.cs" Inherits="Information" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">

        .auto-style5 {
           height: 985px;
        }

        </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div class="auto-style5 w3-dark-grey">
                
      <div class="w3-row-padding" style="margin:0 -16px">
        <div class="w3-half w3-margin-bottom">
          <ul class="w3-ul w3-white w3-center w3-opacity w3-hover-opacity-off">
                  
                  <li class="w3-black w3-xlarge w3-padding-32">Information on users</li>
                    
               <li class="w3-padding-8 ">  <asp:GridView ID="UserGridView" runat="server" Width="100%" CellPadding="3" ForeColor="Black" GridLines="Vertical" BackColor="White" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px">
                        <AlternatingRowStyle BackColor="#CCCCCC" />
                        <FooterStyle BackColor="#CCCCCC" />
                        <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                        <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
                        <SortedAscendingCellStyle BackColor="#F1F1F1" />
                        <SortedAscendingHeaderStyle BackColor="#808080" />
                        <SortedDescendingCellStyle BackColor="#CAC9C9" />
                        <SortedDescendingHeaderStyle BackColor="#383838" />
                    </asp:GridView> </li>
         
          </ul>
        </div>
        
        <div class="w3-half">
          <ul class="w3-ul w3-white w3-center w3-opacity w3-hover-opacity-off">
           
               <li class="w3-black w3-xlarge w3-padding-32">   <asp:TextBox ID="TextBox1" runat="server" Width="209px"></asp:TextBox>
                        <asp:Button ID="SearchFiles" runat="server" Text="Search Files" OnClick="SearchFiles_Click"/>
    </li>
         
              
              
              
                 <li class="w3-padding-8">  <asp:GridView ID="FilesGridView" Width="100%" runat="server" CellPadding="3" ForeColor="Black" GridLines="Vertical" BackColor="White" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px">
                        <AlternatingRowStyle BackColor="#CCCCCC" />
                        <FooterStyle BackColor="#CCCCCC" />
                        <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                        <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
                        <SortedAscendingCellStyle BackColor="#F1F1F1" />
                        <SortedAscendingHeaderStyle BackColor="#808080" />
                        <SortedDescendingCellStyle BackColor="#CAC9C9" />
                        <SortedDescendingHeaderStyle BackColor="#383838" />
                    </asp:GridView></li>
     




          </ul>
        </div>
      </div>

          </div>
</asp:Content>

