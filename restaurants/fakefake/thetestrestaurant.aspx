<%@ Page Language="VB" AutoEventWireup="false" CodeFile="thetestrestaurant.aspx.vb" Inherits="restaurant_thetestrestaurant" %><!DOCTYPE html>
<html class="no-js">
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1">
    <title>the test restaurant | FAKEFAKE</title>
    <meta name="description" content="Test restaurant....">
    <meta name="robots" content="noindex, nofollow">
    <meta name="viewport" content="width=device-width">
    <link rel="stylesheet" href="../_Data/bootstrap/css/bootstrap.min.css">
    <link rel="stylesheet" href="../_Data/bootstrap/css/bootstrap-responsive.min.css">
    <link rel="stylesheet" href="../_Data/themes/font-awesome/css/font-awesome.min.css">
    <link rel="stylesheet" href="../_Data/themes/css/main.css">
    <link rel="stylesheet" href="../_Data/themes/css/prettyPhoto.css">
    <link rel="stylesheet" href="../_Data/themes/css/style.css">
    <link rel="stylesheet" href="../_Data/themes/css/custom-responsive.css">
    <style>
        .upper { text-transform: uppercase; }
    </style>
    <!--[if lt IE 9]>
        <script src="https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js"></script>
        <script src="https://oss.maxcdn.com/libs/respond.js/1.4.2/respond.min.js"></script>
    <![endif]-->
</head>
<body data-spy="scroll" data-target=".navbar">
    <header>
        <div class="container">
            <div class="navbar">
                <div class="navbar-inner">
                    <div class="container">
                        <button type="button" class="btn btn-navbar" data-toggle="collapse" data-target=".nav-collapse">
                            <span class="icon-bar"></span>
                            <span class="icon-bar"></span>
                            <span class="icon-bar"></span>
                        </button>
                        <a class="brand" href="#home"><i class="icon-food"></i>&nbsp; the test restaurant</a>
                        <div class="nav-collapse pull-right in collapse" style="height: auto;">
                            <ul class="nav">
                                <li class="active"><a href="#home">Home</a></li>
                                <li class=""><a href="#contact">Contact</a></li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </header>

    <section id="home" class="fill">
        <div class="container">
            <div class="wrapper">
                <h1 class="upper">WELCOME TO<br>the test restaurant</h1>
                <p class="lead">Test restaurant....</p>
                <a class="btn btn-large btn-chunkfive" href="#contact" id="connect-now"><i class="icon-envelope-alt"></i> Contact</a>
            </div>
        </div>
    </section>

    <section id="contact" class="fill">
        <div class="container">
            <div class="wrapper">
                <h1 class="the-head">Contact</h1>
                <div class="row-fluid">
                    <div class="span6 offset3">
                        <form class="contact-form" runat="server">
                            <fieldset>
                                <table class="table table-striped table-bordered">
                                    <tbody>
                                        <tr>
                                            <th>Address:</th>
                                            <td>123 fake street</td>
                                        </tr>
                                        <tr>
                                            <th>City:</th>
                                            <td>FAKEFAKE</td>
                                        </tr>
                                        <tr>
                                            <th>Opening Time:</th>
                                            <td>18:00</td>
                                        </tr>
                                        <tr>
                                            <th>Closing Time:</th>
                                            <td>21:00</td>
                                        </tr>
                                        <tr>
                                            <th>Post Code:</th>
                                            <td>LE11 2BL</td>
                                        </tr>
                                        <tr>
                                            <th>Email:</th>
                                            <td>info@thetestrestaurant.com</td>
                                        </tr>
                                        <tr>
                                            <th>Phone number:</th>
                                            <td>01234567890</td>
                                        </tr>
                                    </tbody>
                                </table>
                                <div class="control-group">
                                    <asp:Button ID="btnBookNow" runat="server" Text="Book now" CssClass="btn btn-large btn-chunkfive" />
                                    <asp:Button ID="btnBack" runat="server" Text="Go Back" CssClass="btn btn-large btn-chunkfive" />
                                </div>
                            </fieldset>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <a href="#" class="go-top" style="display: none;"><i class="icon-double-angle-up"></i></a>

    <script src="../_Data/themes/js/jquery-1.9.1.min.js"></script>
    <script src="../_Data/themes/js/bootstrap.min.js"></script>
    <script src="../_Data/themes/js/bootstrap-scrollspy.js"></script>
    <script src="../_Data/themes/js/jquery.easing-1.3.min.js"></script>
    <script src="../_Data/themes/js/jquery.scrollTo-1.4.3.1-min.js"></script>
    <script src="../_Data/themes/js/jquery.vegas.js"></script>
    <script src="../_Data/themes/js/sly.min.js"></script>
    <script src="../_Data/themes/js/jquery.prettyPhoto.js"></script>
    <script src="../_Data/themes/js/main.js"></script>
</body>
</html>