<%@ Page Language="VB" AutoEventWireup="false" CodeFile="addrestaurant.aspx.vb" Inherits="owner_addrestaurant" %><!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="description" content="">
    <meta name="author" content="">

    <title>Add New Restaurant | Owner | Restaurant Booking System</title>

    <!-- Bootstrap core CSS -->
    <link href="../css/bootstrap.min.css" rel="stylesheet">

    <!-- Custom styles for this template -->
    <link href="../css/dashboard.css" rel="stylesheet">
    <style>
        .table { width: 100%; }
        .table>thead>tr>th, .table>tbody>tr>th, .table>tfoot>tr>th, .table>thead>tr>td, .table>tbody>tr>td, .table>tfoot>tr>td { border-top: 0 none; }
        .table-condensed>thead>tr>th, .table-condensed>tbody>tr>th, .table-condensed>tfoot>tr>th, .table-condensed>thead>tr>td, .table-condensed>tbody>tr>td, .table-condensed>tfoot>tr>td { padding-left: 0; padding-right: 0; }
        textarea { resize: vertical; }
        #lstTables {
            border-bottom-left-radius: 0;
            border-bottom-right-radius: 0;
        }
        #btnDeleteTable { 
            margin-top: -1px; 
            border-top-left-radius: 0;
            border-top-right-radius: 0;
        }
    </style>

    <!-- HTML5 shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!--[if lt IE 9]>
      <script src="https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js"></script>
      <script src="https://oss.maxcdn.com/libs/respond.js/1.4.2/respond.min.js"></script>
    <![endif]-->
</head>

<body>
    <form id="form1" runat="server">
        <div class="navbar navbar-inverse navbar-fixed-top" role="navigation">
            <div class="container-fluid">
                <div class="navbar-header">
                    <button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-collapse">
                        <span class="sr-only">Toggle navigation</span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                    </button>
                    <asp:Label ID="lblHeader" runat="server" Text="Label" class="navbar-brand"></asp:Label>
                </div>
                <div class="navbar-collapse collapse">
                    <ul class="nav navbar-nav navbar-right">
                        <li><a href="menu.aspx">Dashboard</a></li>
                        <li>
                            <asp:Button ID="btnLogOut" runat="server" Text="Logout" CssClass="btn-to-a" /></li>
                    </ul>
                </div>
            </div>
        </div>

        <div class="container-fluid">
            <div class="row">
                <div class="col-sm-3 col-md-2 sidebar">
                    <ul class="nav nav-sidebar">
                        <li><a href="menu.aspx">Overview</a></li>
                        <li class="active"><a href="addrestaurant.aspx">Add New Restaurant</a></li>
                    </ul>
                    <ul class="nav nav-sidebar">
                        <li><a href="registerstaff.aspx">Register New Staff</a></li>
                    </ul>
                    <ul class="nav nav-sidebar">
                        <li><a href="changepassword.aspx">Change Password</a></li>
                    </ul>
                </div>
                <div class="col-sm-9 col-sm-offset-3 col-md-10 col-md-offset-2 main">
                    <h1 class="page-header">Dashboard</h1>
                    <div class="row placeholders">
                        <div class="col-xs-6 col-sm-3 placeholder">
                            <img src="../images/dashboard/1.jpg" class="img-responsive" alt="Generic placeholder thumbnail">
                        </div>
                        <div class="col-xs-6 col-sm-3 placeholder">
                            <img src="../images/dashboard/2.jpg" class="img-responsive" alt="Generic placeholder thumbnail">
                        </div>
                        <div class="col-xs-6 col-sm-3 placeholder">
                            <img src="../images/dashboard/3.jpg" class="img-responsive" alt="Generic placeholder thumbnail">
                        </div>
                        <div class="col-xs-6 col-sm-3 placeholder">
                            <img src="../images/dashboard/4.jpg" class="img-responsive" alt="Generic placeholder thumbnail">
                        </div>
                    </div>
                    <h2 class="sub-header">Add New Restaurant</h2>
                    <br />
                    <asp:Label ID="Label1" runat="server" Text="Restaurant name:"></asp:Label>
                    <br />
                    <asp:TextBox ID="txtName" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                    <br />
                    <asp:Label ID="Label2" runat="server" Text="Address:"></asp:Label>
                    <br />
                    <asp:TextBox ID="txtAddress" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                    <br />
                    <asp:Label ID="label8" runat="server" Text="City:"></asp:Label>
                    <br />
                    <asp:TextBox ID="txtCity" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox>
                    <br />
                    <asp:Label ID="Label3" runat="server" Text="Opening time: (24 Hours) (HH:MM)"></asp:Label>
                    <br />
                    <asp:TextBox ID="txtOpenTime" runat="server" TextMode="DateTime" MaxLength="5" CssClass="form-control"></asp:TextBox>
                    <br />
                    <asp:Label ID="Label4" runat="server" Text="Closing time: (24 Hours) (HH:MM)"></asp:Label>
                    <br />
                    <asp:TextBox ID="txtCloseTime" runat="server" TextMode="DateTime" MaxLength="5" CssClass="form-control"></asp:TextBox>
                    <br />
                    <asp:Label ID="Label5" runat="server" Text="Post Code:"></asp:Label>
                    <br />
                    <asp:TextBox ID="txtPostcode" runat="server" MaxLength="8" CssClass="form-control"></asp:TextBox>
                    <br />
                    <asp:Label ID="Label6" runat="server" Text="Email:"></asp:Label>
                    <br />
                    <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" MaxLength="50" CssClass="form-control"></asp:TextBox>
                    <br />
                    <asp:Label ID="Label7" runat="server" Text="Phone number:"></asp:Label>
                    <br />
                    <asp:TextBox ID="txtPhoneNo" runat="server" MaxLength="11" CssClass="form-control"></asp:TextBox>
                    <br />
                    <asp:Label ID="Label12" runat="server" Text="Type of food: (seperate multiple types using comma)"></asp:Label>
                    <br />
                    <asp:TextBox ID="txtFoodType" runat="server"  CssClass="form-control"></asp:TextBox>
                    <br />
                    <table class="table table-condensed">
                        <thead>
                            <tr>
                                <td>Table Size</td>
                                <td>Quantity of Table Size</td>
                                <td>&nbsp;</td>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td><asp:DropDownList ID="ddlTableSizes" runat="server"  CssClass="form-control"></asp:DropDownList></td>
                                <td><asp:TextBox ID="txtAmountOfTables" runat="server" TextMode="Number" value="1" min="1"  CssClass="form-control"></asp:TextBox></td>
                                <td style="width:35px;"><asp:Button ID="btnAddtable" runat="server" Text="+"  CssClass="btn" /></td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <asp:ListBox ID="lstTables" runat="server"  CssClass="form-control"></asp:ListBox>
                                    <asp:Button ID="btnDeleteTable" runat="server" Text="Remove Selected Table" CssClass="btn btn-sm btn-default btn-block" />
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <asp:Label ID="Label9" runat="server" Text="Description of restaurant:"></asp:Label>
                    <br />
                    <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine"></asp:TextBox>
                    <br />
                    <br />
                    <asp:Button ID="btnCreate" runat="server" Text="Create restaurant" CssClass="btn btn-lg btn-primary btn-block" />
                    <br />
                    <asp:Label ID="lblSuccess" runat="server" ForeColor="Green" ></asp:Label>
                    <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
                    <br />
                    <br />
                </div>
            </div>
        </div>
    </form>

    <!-- Bootstrap core JavaScript
    ================================================== -->
    <!-- Placed at the end of the document so the pages load faster -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js"></script>
    <script src="../js/bootstrap.min.js"></script>
    <script src="../js/docs.min.js"></script>
</body>
</html>
