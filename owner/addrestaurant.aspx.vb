Imports clsSecurity
Imports System.IO

Partial Class owner_addrestaurant
    Inherits System.Web.UI.Page

    Dim owner As clsOwner
    Dim restaurants As New clsRestaurants

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' has a session been created?
        ' if so restore it otherwise redirect back to login form 
        If Session("owner") IsNot Nothing Then
            owner = Session("owner")
            lblHeader.Text = "Welcome " & owner.FirstName
            PreventBackButtonCaching(Response)
            Form.DefaultButton = btnCreate.UniqueID
        Else
            Response.Redirect("login.aspx")
        End If
    End Sub

    Protected Sub Page_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        ' populate the table sizes listbox 
        ddlTableSizes.DataSource = restaurants.GetTableSizes()
        ddlTableSizes.DataTextField = "NoOfSeats"
        ddlTableSizes.DataValueField = "TableID"
        ddlTableSizes.DataBind()
    End Sub

    Protected Sub Page_Unload(sender As Object, e As EventArgs) Handles Me.Unload
        If Session("owner") IsNot Nothing Then
            Session("owner") = owner
        End If
    End Sub

    Protected Sub btnLogOut_Click(sender As Object, e As EventArgs) Handles btnLogOut.Click
        owner.LogOut() ' fires the LogOut method 
        Session("owner") = Nothing
        Response.Redirect("login.aspx")        ' finally go back to login page
    End Sub

    Protected Sub btnCreate_Click(sender As Object, e As EventArgs) Handles btnCreate.Click
        ' clean and store user input 
        Dim restaurantName As String = StripTagsAndContent(txtName.Text.Trim)
        Dim address As String = StripTagsAndContent(txtAddress.Text.Trim)
        Dim city As String = StripTagsAndContent(txtCity.Text.Trim)
        Dim openTime As DateTime
        Dim closeTime As DateTime
        Dim postCode As String = StripTagsAndContent(txtPostcode.Text.Trim)
        Dim email As String = StripTagsAndContent(txtEmail.Text.Trim)
        Dim phoneNo As String = StripTagsAndContent(txtPhoneNo.Text.Trim)
        Dim foodType As String = StripTagsAndContent(txtFoodType.Text.Trim)
        Dim description As String = StripTagsAndContent(txtDescription.Text.Trim)
        Dim errors As New List(Of String)   ' store error messages 

        ' check if the data entered is valid, if not we store the error messages 
        lblError.Text = ""
        lblSuccess.Text = ""
        If String.IsNullOrWhiteSpace(restaurantName) Then errors.Add("Please enter a restaurant name.")
        If String.IsNullOrWhiteSpace(address) Then errors.Add("Please enter the address of the restaurant.")
        If String.IsNullOrWhiteSpace(city) Then errors.Add("Please enter the city that the restaurant is located in.")
        If Not isValidPostCode(postCode) Then errors.Add("Please enter a valid postcode.")
        If String.IsNullOrWhiteSpace(phoneNo) And phoneNo.Length <> 11 Then errors.Add("Please enter the restaurant mobile number correctly.")
        If String.IsNullOrWhiteSpace(foodType) Then errors.Add("Please enter the type of food the restaurant serves.")
        If String.IsNullOrWhiteSpace(description) Then errors.Add("Please enter a description of the restaurant.")
        If Not isValidEmail(email) Then errors.Add("Please enter a valid email address.")

        If DateTime.TryParse(txtOpenTime.Text, Nothing) And (txtOpenTime.Text.Length = 5) Then
            If txtOpenTime.Text.Substring(txtOpenTime.Text.Length - 2) = "00" Then
                openTime = DateTime.Parse(txtOpenTime.Text)
            Else
                errors.Add("Opening times need to be hourly increments.")
            End If
        Else
            errors.Add("Please select a valid opening time.")
        End If

        If DateTime.TryParse(txtCloseTime.Text, Nothing) And (txtCloseTime.Text.Length = 5) Then
            If txtCloseTime.Text.Substring(txtCloseTime.Text.Length - 2) = "00" Then
                If (Convert.ToInt32(txtCloseTime.Text.Substring(0, 2)) > Convert.ToInt32(txtOpenTime.Text.Substring(0, 2))) And (Convert.ToInt32(txtCloseTime.Text.Substring(0, 2)) <= 24) Then
                    closeTime = DateTime.Parse(txtCloseTime.Text)
                Else
                    errors.Add("Closing time needs to be after the opening time and before midnight.")
                End If
            Else
                errors.Add("Closing times need to be hourly increments.")
            End If
        Else
            errors.Add("Please select a valid closing time.")
        End If


        If lstTables.Items.Count = 0 Then
            errors.Add("There are no tables specified.")
        End If

        ' if the error array contains nothing, then all data is valid 
        If errors.Count = 0 Then
            Dim cityNameSmall As String
            Dim restNameSmall As String
            Dim aspPath As String
            Dim vbPath As String
            Dim homeDir As String = AppDomain.CurrentDomain.BaseDirectory
            Dim rawPath As String

            ' get the url of the new page that will be made so it can be stored into the db 
            cityNameSmall = RemoveCharsBesidesLetters(city).Trim.ToLower
            restNameSmall = RemoveCharsBesidesLetters(restaurantName).Trim.ToLower

            rawPath = "restaurants\" & cityNameSmall & "\" & restNameSmall & ".aspx"
            aspPath = homeDir & rawPath
            vbPath = aspPath & ".vb"

            ' get all the tables that the restaurant has, parse them & store them 
            Dim tables As New List(Of String)
            Dim totalTables As Integer = lstTables.Items.Count
            For i = 0 To totalTables - 1
                tables.Add(lstTables.Items(i).Value)
            Next

            ' lets add the restaurant & table data into the DB
            If owner.AddNewRestaurant(owner.ID, restaurantName, address, city, openTime, closeTime, postCode, email, phoneNo, foodType, tables, rawPath) Then
                ' if successfull, lets create a page for the owner
                ' read templates in
                Dim websiteTemplate As String = File.ReadAllText(Server.MapPath("../Resources/website_gen_template.txt"))
                Dim vbTemplate As String = File.ReadAllText(Server.MapPath("../Resources/website_vb_template.txt"))

                ' replace keywords with the actual data from user input
                websiteTemplate = websiteTemplate.Replace("[codefile]", restNameSmall & ".aspx.vb")
                websiteTemplate = websiteTemplate.Replace("[inherits]", "restaurant_" & restNameSmall)
                websiteTemplate = websiteTemplate.Replace("[restName]", restaurantName)
                websiteTemplate = websiteTemplate.Replace("[address]", address)
                websiteTemplate = websiteTemplate.Replace("[city]", city.ToUpper)
                websiteTemplate = websiteTemplate.Replace("[openTime]", openTime.ToString("HH:mm"))
                websiteTemplate = websiteTemplate.Replace("[closeTime]", closeTime.ToString("HH:mm"))
                websiteTemplate = websiteTemplate.Replace("[postCode]", postCode)
                websiteTemplate = websiteTemplate.Replace("[email]", email)
                websiteTemplate = websiteTemplate.Replace("[phoneNo]", phoneNo)
                websiteTemplate = websiteTemplate.Replace("[description]", description)
                vbTemplate = vbTemplate.Replace("[inherits]", "restaurant_" & restNameSmall)

                ' if directories don't exist, create them
                If Not Directory.Exists(homeDir & "restaurants\" & cityNameSmall & "\") Then
                    Directory.CreateDirectory(homeDir & "restaurants\" & cityNameSmall & "\")
                End If

                ' write new contents into new aspx file
                File.WriteAllText(aspPath, websiteTemplate)
                File.WriteAllText(vbPath, vbTemplate)

                'display success message
                lblSuccess.Text = "Success!"
            Else
                lblError.Text = "Please check all data is in correct format!"
            End If
        Else
            ' otherwise print all errors out
            Dim totalErrors As Integer = errors.Count
            For i = 0 To totalErrors - 1
                lblError.Text &= errors(i) & "<br>"
            Next
        End If
    End Sub

    Protected Sub btnAddtable_Click(sender As Object, e As EventArgs) Handles btnAddtable.Click
        Dim tableSize As Integer
        Dim amountOfTable As Integer
        Dim outputText As String
        Dim outputValue As String

        ' if a table size has been selected
        If ddlTableSizes.SelectedIndex <> -1 Then
            ' is the amount an integer > 0?
            If IsNumeric(txtAmountOfTables.Text) And txtAmountOfTables.Text > 0 Then
                ' store the data
                tableSize = ddlTableSizes.SelectedItem.ToString
                amountOfTable = txtAmountOfTables.Text
                ' build the output strings
                outputText = String.Format("You have {0} {1} seater tables", amountOfTable, tableSize)
                outputValue = String.Format("{0},{1}", ddlTableSizes.SelectedValue, amountOfTable)
                ' add the data to the listbox 
                lstTables.Items.Add(New ListItem(outputText, outputValue))
            Else
                lblError.Text = "Please enter a number that is greater than 0."
            End If
        Else
            lblError.Text = "Please select a table size."
        End If
    End Sub

    Protected Sub btnDeleteTable_Click(sender As Object, e As EventArgs) Handles btnDeleteTable.Click
        lblError.Text = ""
        If lstTables.SelectedIndex <> -1 Then
            lstTables.Items.RemoveAt(lstTables.SelectedIndex)
        Else
            lblError.Text = "Please add a table into the tables list."
        End If
    End Sub

    Private Function RemoveCharsBesidesLetters(str As String) As String
        Return Regex.Replace(str, "[^a-zA-Z]+", "")
    End Function
End Class
