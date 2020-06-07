<%@ Page Language="VB" AutoEventWireup="false" CodeFile="login.aspx.vb" Inherits="login" %><!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="description" content="">
    <meta name="author" content="">

    <title>Member Login | Restaurant Booking System</title>

    <!-- Bootstrap core CSS -->
    <link href="../css/bootstrap.min.css" rel="stylesheet">

    <!-- Custom styles for this template -->
    <link href="../css/signin.css" rel="stylesheet">
    <!--[if IE 9]><link href="../css/ie9.css" rel="stylesheet"><![endif]-->
    <style>
        .placeholder { color: #999; }
    </style>

    <!-- HTML5 shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!--[if lt IE 9]>
      <script src="https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js"></script>
      <script src="https://oss.maxcdn.com/libs/respond.js/1.4.2/respond.min.js"></script>
    <![endif]-->
</head>

<body>

    <div class="container">
        <form id="form1" class="form-signin" role="form" runat="server">
            <h2 class="form-signin-heading">Member login:</h2>
            <asp:TextBox ID="txtEmail" runat="server" placeholder="Email address" required autofocus TextMode="Email" CssClass="form-control"></asp:TextBox>
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Password" required></asp:TextBox>
            <asp:Button ID="btnLogin" runat="server" Text="Sign in" CssClass="btn btn-lg btn-primary btn-block" />
            <br />
            <a href="register.aspx">Don't have an account? Click here to register</a>
            <br />
            <a href="../Default.aspx">View restaurants.</a>
            <br />
            <br />
            <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
        </form>
    </div>

    <!-- Bootstrap core JavaScript
    ================================================== -->
    <!-- Placed at the end of the document so the pages load faster -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js"></script>
    <script src="../js/jquery.placeholder.min.js"></script>
    <script>
        $(function () {
            $('input, textarea').placeholder();
        });
    </script>
</body>
</html>
