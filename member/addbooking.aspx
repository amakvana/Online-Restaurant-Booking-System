<%@ Page Language="VB" AutoEventWireup="false" CodeFile="addbooking.aspx.vb" Inherits="member_addbooking" %><!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="description" content="">
    <meta name="author" content="">

    <title>Member Home | Restaurant Booking System</title>

    <!-- Bootstrap core CSS -->
    <link href="../css/bootstrap.min.css" rel="stylesheet">

    <!-- Custom styles for this template -->
    <link href="../css/dashboard.css" rel="stylesheet">
    <!--[if IE 9]><link href="../css/ie9.css" rel="stylesheet"><![endif]-->

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
                        <li class="active"><a href="addbooking.aspx">Add New Booking</a></li>
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
                    <h2 class="sub-header">Add New Booking</h2>
                    <br />
                    <asp:Label ID="Label2" runat="server" Text="Restaurant City:"></asp:Label>
                    <br />
                    <asp:DropDownList ID="ddlRestCity" runat="server" AutoPostBack="True" CssClass="form-control">
                    </asp:DropDownList>
                    <br />
                    <asp:Label ID="Label3" runat="server" Text="Restaurant Name:"></asp:Label>
                    <br />
                    <asp:DropDownList ID="ddlRestName" runat="server" AutoPostBack="True" CssClass="form-control">
                    </asp:DropDownList>
                    <br />
                    <asp:Label ID="Label4" runat="server" Text="Booking Date: (DD/MM/YYYY)"></asp:Label>
                    <br />
                    <asp:TextBox ID="txtDate" runat="server" MaxLength="10" TextMode="Date" CssClass="form-control"></asp:TextBox>

                    <br />
                    <asp:Label ID="Label5" runat="server" Text="Booking Time:"></asp:Label>
                    <br />
                    <asp:DropDownList ID="ddlTime" runat="server" CssClass="form-control">
                    </asp:DropDownList>
                    <br />
                    <asp:Label ID="Label6" runat="server" Text="Table Size:"></asp:Label>
                    <br />
                    <asp:DropDownList ID="ddlTableSize" runat="server" CssClass="form-control">
                    </asp:DropDownList>
                    <br />
                    <asp:Label ID="Label7" runat="server" Text="Number of People:"></asp:Label>
                    <br />
                    <asp:TextBox ID="txtNoOfPeople" runat="server" max="12" min="2" TextMode="Number" CssClass="form-control">2</asp:TextBox>
                    <br />
                    <br />
                    <asp:Button ID="btnAdd" runat="server" Text="Add Booking" CssClass="btn btn-lg btn-primary btn-block" />
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
