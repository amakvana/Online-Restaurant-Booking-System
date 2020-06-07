<%@ Page Language="VB" AutoEventWireup="false" CodeFile="view.aspx.vb" Inherits="restaurants_view" %><!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="description" content="">
    <meta name="author" content="">

    <title>Restaurant Booking System</title>

    <!-- Bootstrap core CSS -->
    <link href="../css/bootstrap.min.css" rel="stylesheet">

    <!-- Custom styles for this template -->
    <link href="../css/general-content.css" rel="stylesheet">

    <!-- HTML5 shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!--[if lt IE 9]>
      <script src="https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js"></script>
      <script src="https://oss.maxcdn.com/libs/respond.js/1.4.2/respond.min.js"></script>
    <![endif]-->
</head>
<body>

    <div class="container">
        <form id="form1" runat="server" class="form-panel-default">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title">
                        <asp:Label ID="lblHeader" runat="server" Text="Select an Area"></asp:Label>
                        <a href="../member/login.aspx">Sign in.</a>
                    </h3>
                </div>
                <div class="panel-body">
                    <asp:Label ID="Label1" runat="server" Text="Now select a restaurant:"></asp:Label>
                    <br />
                    <br />
                    <div id="restaurantNames" runat="server"> </div>
                    <br />
                    <asp:Button ID="btnBack" runat="server" Text="Go Back" CssClass="btn btn-default btn-block" />
                </div>
            </div>
        </form>
    </div>

    <!-- Bootstrap core JavaScript
    ================================================== -->
    <!-- Placed at the end of the document so the pages load faster -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js"></script>
    <script src="../js/bootstrap.min.js"></script>
    <script src="../js/docs.min.js"></script>
</body>
</html>
