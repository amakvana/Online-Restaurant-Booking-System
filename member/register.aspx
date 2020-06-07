<%@ Page Language="VB" AutoEventWireup="false" CodeFile="register.aspx.vb" Inherits="member_register" %><!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="description" content="">
    <meta name="author" content="">

    <title>Member Registration | Restaurant Booking System</title>

    <!-- Bootstrap core CSS -->
    <link href="../css/bootstrap.min.css" rel="stylesheet">

    <!-- Custom styles for this template -->
    <link href="../css/general-content.css" rel="stylesheet">
    <link href="../css/registration.css" rel="stylesheet">
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
        <form id="form1" runat="server" class="form-panel-default" role="form">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title">
                        <asp:Label ID="lblHeader" runat="server" Text="Member Registration"></asp:Label>
                    </h3>
                </div>
                <div class="panel-body">
                    <div class="row">
			    		<div class="col-xs-6 col-sm-6 col-md-6">
			    			<div class="form-group">
                                <asp:TextBox ID="txtFirstname" runat="server" MaxLength="50" class="form-control" placeholder="First Name"></asp:TextBox>
			    			</div>
			    		</div>
			    		<div class="col-xs-6 col-sm-6 col-md-6">
			    			<div class="form-group">
                                <asp:TextBox ID="txtSurname" runat="server" MaxLength="50" class="form-control" placeholder="Surname"></asp:TextBox>
			    			</div>
			    		</div>
			    	</div>

                    <div class="form-group">
                        <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" MaxLength="50" class="form-control" placeholder="Email Address"></asp:TextBox>
			    	</div>

			    	<div class="row">
			    		<div class="col-xs-6 col-sm-6 col-md-6">
			    			<div class="form-group">
                                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" class="form-control" placeholder="Password"></asp:TextBox>
			    			</div>
			    		</div>
			    		<div class="col-xs-6 col-sm-6 col-md-6">
			    			<div class="form-group">
                                <asp:TextBox ID="txtVerifyPass" runat="server" TextMode="Password" class="form-control" placeholder="Confirm Password"></asp:TextBox>
			    			</div>
			    		</div>
			    	</div>
                    <asp:Button ID="btnCreate" runat="server" Text="Register" class="btn btn-info btn-block" />
                    <asp:Button ID="btnBack" runat="server" Text="Go Back" class="btn btn-default btn-block" />
                    <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
                </div>
            </div>
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
