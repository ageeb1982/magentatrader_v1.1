$(document).ready(function() {
    var Sales;

    var SalesGrid;

    var SaleDate;
    var SaleRenewalDate;
    var SaleExpiryDate;

    var ax;
    var ac;

    var btnFirstPageSale;
    var btnPreviousPageSale;
    var btnNextPageSale;
    var btnLastPageSale;
    var btnCurrentPageSale;

    var UserList;
    var ProductPackageList;

    function CmdSaleEdit_OnClick() {
        Sales.editItem(Sales.currentItem);

        $('#SaleEdit').modal({
            show: true,
            backdrop: false
        });

        var Sale = Sales.currentEditItem;
        document.getElementById('SaleEdit_Id').value = Sale.Id !== null && typeof (Sale.Id) != 'undefined' ? wijmo.Globalize.format(Sale.Id) : '';
        document.getElementById('SaleEdit_ProductPackageId').value = Sale.ProductPackageId ? Sale.ProductPackageId : '';
        document.getElementById('SaleEdit_UserId').value = Sale.UserId ? Sale.UserId : '';
        document.getElementById('SaleEdit_SalesNumber').value = Sale.SalesNumber ? Sale.SalesNumber : '';
        document.getElementById('SaleEdit_SaleDate').value = Sale.SalesDate ? Sale.SalesDate : '';
        document.getElementById('SaleEdit_SaleDate_Data').value = Sale.SalesDate ? Sale.SalesDate : '';
        document.getElementById('SaleEdit_RenewalDate').value = Sale.RenewalDate ? Sale.RenewalDate : '';
        document.getElementById('SaleEdit_RenewalDate_Data').value = Sale.RenewalDate ? Sale.RenewalDate : '';
        document.getElementById('SaleEdit_ExpiryDate').value = Sale.ExpiryDate ? Sale.ExpiryDate : '';
        document.getElementById('SaleEdit_ExpiryDate_Data').value = Sale.ExpiryDate ? Sale.ExpiryDate : '';
        document.getElementById('SaleEdit_Particulars').value = Sale.Particulars ? Sale.Particulars : '';
        document.getElementById('SaleEdit_Quantity').value = Sale.Quantity ? Sale.Quantity : '';
        document.getElementById('SaleEdit_Price').value = Sale.Price ? Sale.Price : '';
        document.getElementById('SaleEdit_Amount').value = Sale.Amount ? Sale.Amount : '';
        document.getElementById('SaleEdit_IsActive').checked = Sale.IsActive;
        document.getElementById('SaleEdit_IsRefunded').checked = Sale.IsRefunded;


        SaleDate.dispose();
        SaleDate = new wijmo.input.InputDate('#SaleEdit_SaleDate', {
            format: 'M/d/yyyy',
            value: new Date(Sale.SaleDate),
            onValueChanged: function () {
                document.getElementById('SaleEdit_SaleDate_Data').value = this.value.toString("M/d/yyyy");
            }
        });

        RenewalDate.dispose();
        RenewalDate = new wijmo.input.InputDate('#SaleEdit_RenewalDate', {
            format: 'M/d/yyyy',
            value: new Date(Sale.RenewalDate),
            onValueChanged: function () {
                document.getElementById('SaleEdit_RenewalDate_Data').value = this.value.toString("M/d/yyyy");
            }
        });

        ExpiryDate.dispose();
        ExpiryDate = new wijmo.input.InputDate('#SaleEdit_ExpiryDate', {
            format: 'M/d/yyyy',
            value: new Date(Sale.ExpiryDate),
            onValueChanged: function () {
                document.getElementById('SaleEdit_ExpiryDate_Data').value = this.value.toString("M/d/yyyy");
            }
        });

        ac.dispose();
        ac = new wijmo.input.AutoComplete('#cboSaleEdit_UserId', {
            itemsSource: getUsersList(UserList.items),
            selectedValue: Sale.User,
            onSelectedIndexChanged: function () {
                $("#SaleEdit_UserId").val(UserList.items[this.selectedIndex].Id);
            }
        });
        ax.dispose();
        ax = new wijmo.input.AutoComplete('#cboSaleEdit_ProductPackageId', {
            itemsSource: getProductPackageList(ProductPackageList.items),
            selectedValue: Sale.ProductPackage,
            onSelectedIndexChanged: function () {
                $("#SaleEdit_ProductPackageId").val(ProductPackageList.items[this.selectedIndex].Id);
            }
        });
    }
    function CmdSaleAdd_OnClick() {

        $('#SaleEdit').modal({
            show: true,
            backdrop: false
        });

        var CurrentDate = new Date();

        document.getElementById('SaleEdit_Id').value = 0;
        document.getElementById('SaleEdit_ProductPackageId').value = ProductPackageList.items[0].Id;
        document.getElementById('SaleEdit_UserId').value = UserList.items[0].Id;
        document.getElementById('SaleEdit_SalesNumber').value = '';
        document.getElementById('SaleEdit_SaleDate').value = CurrentDate.toString("M/d/yyyy");
        document.getElementById('SaleEdit_SaleDate_Data').value = CurrentDate.toString("M/d/yyyy");
        document.getElementById('SaleEdit_RenewalDate').value = CurrentDate.toString("M/d/yyyy");;
        document.getElementById('SaleEdit_RenewalDate_Data').value = CurrentDate.toString("M/d/yyyy");
        document.getElementById('SaleEdit_ExpiryDate').value = CurrentDate.toString("M/d/yyyy");
        document.getElementById('SaleEdit_ExpiryDate_Data').value = CurrentDate.toString("M/d/yyyy");
        document.getElementById('SaleEdit_Particulars').value = '';
        document.getElementById('SaleEdit_Quantity').value = 1;
        document.getElementById('SaleEdit_Price').value = 0;
        document.getElementById('SaleEdit_Amount').value = '';
        document.getElementById('SaleEdit_Quantity').addEventListener('keyup', function () {
            document.getElementById('SaleEdit_Amount').value = document.getElementById('SaleEdit_Quantity').value * document.getElementById('SaleEdit_Price').value;
        });
        document.getElementById('SaleEdit_Price').addEventListener('keyup', function () {
            document.getElementById('SaleEdit_Amount').value = document.getElementById('SaleEdit_Quantity').value * document.getElementById('SaleEdit_Price').value;
        });
        document.getElementById('SaleEdit_IsActive').checked = false;
        document.getElementById('SaleEdit_IsRefunded').checked = false;

        SaleDate.dispose();
        SaleDate = new wijmo.input.InputDate('#SaleEdit_SaleDate', {
            format: 'M/d/yyyy',
            value: new Date(Sales.SaleDate),
            onValueChanged: function () {
                document.getElementById('SaleEdit_SaleDate_Data').value = this.value.toString("M/d/yyyy");
            }
        });

        RenewalDate.dispose();
        RenewalDate = new wijmo.input.InputDate('#SaleEdit_RenewalDate', {
            format: 'M/d/yyyy',
            value: new Date(Sales.RenewalDate),
            onValueChanged: function () {
                document.getElementById('SaleEdit_RenewalDate_Data').value = this.value.toString("M/d/yyyy");
            }
        });

        ExpiryDate.dispose();
        ExpiryDate = new wijmo.input.InputDate('#SaleEdit_ExpiryDate', {
            format: 'M/d/yyyy',
            value: new Date(Sales.ExpiryDate),
            onValueChanged: function () {
                document.getElementById('SaleEdit_ExpiryDate_Data').value = this.value.toString("M/d/yyyy");
            }
        });

        ac.dispose();
        ac = new wijmo.input.AutoComplete('#cboSaleEdit_UserId', {
            itemsSource: getUsersList(UserList.items),
            placeholder: 'Select User ',
            onSelectedIndexChanged: function () {

                $("#SaleEdit_UserId").val(UserList.items[this.selectedIndex].Id);
            }
        });
        ax.dispose();
        ax = new wijmo.input.AutoComplete('#cboSaleEdit_ProductPackageId', {
            itemsSource: getProductPackageList(ProductPackageList.items),
            placeholder: 'Select Product Package',
            onSelectedIndexChanged: function () {

                $("#SaleEdit_ProductPackageId").val(ProductPackageList.items[this.selectedIndex].Id);
            }
        });
    }
    function CmdSaleDelete_OnClick() {
        Sales.editItem(Sales.currentItem);

        var Id = Sales.currentEditItem.Id;
        var SalesNumber = Sales.currentEditItem.SalesNumber;

        if (confirm("Delete Sales Number: " + SalesNumber + "?") == true) {
            $.ajax({
                type: "DELETE",
                url: "/api/DeleteSales/" + Id,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                statusCode: {
                    200: function () {
                        window.location.reload();
                    },
                    404: function () {
                        alert("Not found");
                    },
                    400: function () {
                        alert("Bad request");
                    }
                }
            });
        }
    }
    function CmdSaleEditOk_OnClick() {
        if (confirm("Save Sales?") == true) {

            var Sale = new Object();

            Sale.Id = document.getElementById('SaleEdit_Id').value;
            Sale.ProductPackageId = document.getElementById('SaleEdit_ProductPackageId').value;
            Sale.UserId = document.getElementById('SaleEdit_UserId').value;
            Sale.SalesNumber = document.getElementById('SaleEdit_SalesNumber').value;
            Sale.SalesDate = document.getElementById('SaleEdit_SaleDate_Data').value;
            Sale.RenewalDate = document.getElementById('SaleEdit_RenewalDate_Data').value;
            Sale.ExpiryDate = document.getElementById('SaleEdit_ExpiryDate_Data').value;
            Sale.Particulars = document.getElementById('SaleEdit_Particulars').value;
            Sale.Quantity = document.getElementById('SaleEdit_Quantity').value;
            Sale.Price = document.getElementById('SaleEdit_Price').value;
            Sale.Amount = document.getElementById('SaleEdit_Amount').value;
            Sale.IsActive = document.getElementById('SaleEdit_IsActive').checked;
            Sale.IsRefunded = document.getElementById('SaleEdit_IsRefunded').checked;


            var data = JSON.stringify(Sale)
            // Add New
            if (Sale.Id == 0) {
                $.ajax({
                    type: "POST",
                    url: "/api/AddSales",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: data,
                    success: function (id) {
                        if (id > 0) {
                            window.location.reload();
                        } else {
                            alert("Not added");
                        }
                    }
                });

                // Edit
            } else {
                $.ajax({
                    type: "PUT",
                    url: "/api/UpdateSales/" + Sale.Id,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: data,
                    statusCode: {
                        200: function () {
                            window.location.reload();
                        },
                        404: function () {
                            alert("Not found");
                        },
                        400: function () {
                            alert("Bad request");
                        }
                    }
                });
            }
        }
    }

    //function CmdLedgerView_OnClick(Id) {
    //    window.location.href = '/Admin/PackageLedger?product=' + Id;
    //}

    function GetSales() {
        var sales = new wijmo.collections.ObservableArray();
        $('#loading').modal('show');
        $.ajax({
            url: '/api/Sales',
            cache: false,
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            data: {},
            success: function (Results) {
                $('#loading').modal('hide');
                if (Results.length > 0) {
                    for (i = 0; i < Results.length; i++) {
                        sales.push({
                            EditId: "<button class='btn btn-primary btn-xs' data-toggle='modal' id='CmdEditSale' onclick='CmdSaleEdit_OnClick()'>Edit</button>",
                            //LedgerId: "<button class='btn btn-success btn-xs' data-toggle='modal' id=" + Results[i]["ProductPackageId"] + " onclick='CmdLedgerView_OnClick(this.id)'>View</button>",
                            DeleteId: "<button class='btn btn-danger btn-xs' data-toggle='modal' id='CmdDeleteSale' onclick='CmdSaleDelete_OnClick()'>Delete</button>",
                            Id: Results[i]["Id"],
                            ProductPackageId: Results[i]["ProductPackageId"],
                            ProductPackage: Results[i]["ProductPackage"],
                            UserId: Results[i]["UserId"],
                            User: Results[i]["User"],
                            FirstName: Results[i]["FirstName"],
                            LastName: Results[i]["LastName"],
                            SalesNumber: Results[i]["SalesNumber"],
                            SalesDate: Results[i]["SalesDate"],
                            RenewalDate: Results[i]["RenewalDate"],
                            ExpiryDate: Results[i]["ExpiryDate"],
                            Particulars: Results[i]["Particulars"],
                            Quantity: Results[i]["Quantity"],
                            Price: Results[i]["Price"],
                            Amount: Results[i]["Amount"],
                            IsActive: Results[i]["IsActive"],
                            IsRefunded: Results[i]["IsRefunded"]
                        });
                    }
                } else {
                    alert("No data.");
                }
            }
        }).fail(
            function (xhr, textStatus, err) {
                alert(err);
            }
        );
        return sales;
    }
    function getUsersInfo(id) {
        var retItem;

        for (var i = 0; i < UserList.items.length; i++) {
            if (UserList.items[i].Id == id)
                retItem = UserList.items[i].User;
        }
        return retItem;
    }
    function getProductPackageInfo(id) {
        var retItem;

        for (var i = 0; i < ProductPackageList.items.length; i++) {
            if (ProductPackageList.items[i].Id == id)
                retItem = ProductPackageList.items[i].ProductPackageDescription;
        }

        return retItem;
    }
    function getUsersList(items) {
        var retList = new Array();

        for (var i = 0; i < items.length; i++) {
            retList.push(items[i].UserName);
        }

        return retList;
    }
    function getProductPackageList(items) {

        var retList = new Array();

        for (var i = 0; i < items.length; i++) {
            retList.push(items[i].ProductPackageDescription);
        }

        return retList;
    }
    function getUsers() {
        var users = new wijmo.collections.ObservableArray();
        $.ajax({
            url: '/api/User',
            cache: false,
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            data: {},
            success: function (Results) {
                if (Results.length > 0) {
                    for (i = 0; i < Results.length; i++) {
                        users.push({
                            Id: Results[i]["Id"],
                            UserName: Results[i]["UserName"],
                            FirstName: Results[i]["FirstName"],
                            LastName: Results[i]["LastName"],
                            EmailAddress: Results[i]["EmailAddress"],
                            PhoneNumber: Results[i]["PhoneNumber"]
                        });
                    }
                } else {
                    // alert("No data.");
                }
            }
        }).fail(
            function (xhr, textStatus, err) {
                alert(err);
            }
        );
        return users;
    }
    function getProductPackage() {
        var productPackage = new wijmo.collections.ObservableArray();
        $.ajax({
            url: '/api/ProductPackage',
            cache: false,
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            data: {},
            success: function (Results) {
                if (Results.length > 0) {
                    for (i = 0; i < Results.length; i++) {
                        productPackage.push({
                            Id: Results[i]["Id"],
                            ProductPackageDescription: Results[i]["ProductPackageDescription"]
                        });
                    }
                } else {
                    // alert("No data.");
                }
            }
        }).fail(
            function (xhr, textStatus, err) {
                alert(err);
            }
        );
        return productPackage;
    }

    function UpdateNavigateButtonsSale() {
        if (Sales.pageSize <= 0) {
            document.getElementById('naviagtionPageSale').style.display = 'none';
            return;
        }
        document.getElementById('naviagtionPageSale').style.display = 'block';
        if (Sales.pageIndex === 0) {
            btnFirstPageSale.setAttribute('disabled', 'disabled');
            btnPreviousPageSale.setAttribute('disabled', 'disabled');
            btnNextPageSale.removeAttribute('disabled');
            btnLastPageSale.removeAttribute('disabled');
        }
        else if (Sales.pageIndex === (Sales.pageCount - 1)) {
            btnFirstPageSale.removeAttribute('disabled');
            btnPreviousPageSale.removeAttribute('disabled');
            btnLastPageSale.setAttribute('disabled', 'disabled');
            btnNextPageSale.setAttribute('disabled', 'disabled');
        }
        else {
            btnFirstPageSale.removeAttribute('disabled');
            btnPreviousPageSale.removeAttribute('disabled');
            btnNextPageSale.removeAttribute('disabled');
            btnLastPageSale.removeAttribute('disabled');
        }
        btnCurrentPageSale.innerHTML = (Sales.pageIndex + 1) + ' / ' + Sales.pageCount;
    }

    //$(document).ready(function () {

        btnFirstPageSale = document.getElementById('btnMoveToFirstPageSale');
        btnPreviousPageSale = document.getElementById('btnMoveToPreviousPageSale');
        btnNextPageSale = document.getElementById('btnMoveToNextPageSale');
        btnLastPageSale = document.getElementById('btnMoveToLastPageSale');
        btnCurrentPageSale = document.getElementById('btnCurrentPageSale');

        Sales = new wijmo.collections.CollectionView(GetSales());
        UserList = new wijmo.collections.CollectionView(getUsers());
        ProductPackageList = new wijmo.collections.CollectionView(getProductPackage());

        Sales.canFilter = true;

        var DateFilter = new wijmo.input.InputDate('#SalesDateFilter', {
            format: 'M/d/yyyy',
            value: new Date()
        });

        var filterElement = document.getElementById('SalesDateFilter'),
            filterText = $('#SalesDateFilter').find('input').val();


        $('#SalesDateFilter').on('click', function () {
            var str = $('#SalesDateFilter').find('input').val();
            filterText = str;
            Sales.refresh();
        });

        Sales.filter = function (item) {
            return !filterText || item.SalesDate.indexOf(filterText) > -1;
        };

        //SalesGrid = new wijmo.grid.FlexGrid('#SalesGrid');

        //SalesGrid.initialize({
        //    columns: [
        //        {
        //            "header": "Edit",
        //            "binding": "EditId",
        //            "width": 60,
        //            "allowSorting": false,
        //            "isContentHtml": true
        //        },
        //        //{
        //        //    "header": "Ledger",
        //        //    "binding": "LedgerId",
        //        //    "width": 60,
        //        //    "allowSorting": false,
        //        //    "isContentHtml": true
        //        //},
        //        {
        //            "header": "Delete",
        //            "binding": "DeleteId",
        //            "width": 60,
        //            "allowSorting": false,
        //            "isContentHtml": true
        //        },
        //        {
        //            "header": "Sales Number",
        //            "binding": "SalesNumber",
        //            "allowSorting": false,
        //            "width": 110
        //        },
        //         {
        //             "header": "Username",
        //             "binding": "User",
        //             "allowSorting": false,
        //             "width": "2*"
        //         },
        //        {
        //            "header": "Product Package",
        //            "binding": "ProductPackage",
        //            "allowSorting": false,
        //            "width": "4*",
        //            "wordWrap": false
        //        },
        //        {
        //            "header": "Sale Date",
        //            "binding": "SalesDate",
        //            "allowSorting": false,
        //            "width": 100,
        //            "visible": false
        //        },
        //        {
        //            "header": "Renewal Date",
        //            "binding": "RenewalDate",
        //            "allowSorting": false,
        //            "width": 100,
        //            "visible": false
        //        },
        //        {
        //            "header": "Expiry Date",
        //            "binding": "ExpiryDate",
        //            "allowSorting": false,
        //            "width": 100,
        //            "visible": false
        //        },
        //         {
        //             "header": "Particulars",
        //             "binding": "Particulars",
        //             "allowSorting": false,
        //             "width": "4*",
        //             "visible": false
        //         },

        //        {
        //            "header": "Quantity",
        //            "binding": "Quantity",
        //            "allowSorting": false,
        //            "width": 80
        //        },
        //        {
        //            "header": "Price",
        //            "binding": "Price",
        //            "allowSorting": false,
        //            "width": 80
        //        },
        //         {
        //             "header": "Amount",
        //             "binding": "Amount",
        //             "allowSorting": false,
        //             "width": 80
        //         }
        //    ],
        //    autoGenerateColumns: false,
        //    itemsSource: Sales,
        //    isReadOnly: true,
        //    selectionMode: wijmo.grid.SelectionMode.Row
        //});

        //SalesGrid.trackChanges = true;

        //Sales.pageSize = 15;

        SaleDate = new wijmo.input.InputDate('#SaleEdit_SaleDate', {
            format: 'M/d/yyyy',
            value: new Date()
        });

        RenewalDate = new wijmo.input.InputDate('#SaleEdit_RenewalDate', {
            format: 'M/d/yyyy',
            value: new Date()
        });

        ExpiryDate = new wijmo.input.InputDate('#SaleEdit_ExpiryDate', {
            format: 'M/d/yyyy',
            value: new Date()
        });

        ac = new wijmo.input.AutoComplete('#cboSaleEdit_UserId', {
            itemsSource: getUsersList(UserList.items),
            placeholder: 'Select User ',
            onSelectedIndexChanged: function (e, data) {
                $("#SaleEdit_UserId").val(UserList.items[this.selectedIndex].Id);
            }
        });

        ax = new wijmo.input.AutoComplete('#cboSaleEdit_ProductPackageId', {
            itemsSource: getProductPackageList(ProductPackageList.items),
            placeholder: 'Select Product Package ',
            onSelectedIndexChanged: function (e, data) {
                $("#SaleEdit_ProductPackageId").val(ProductPackageList.items[this.selectedIndex].Id);
            }
        });

        UpdateNavigateButtonsSale();

        // Page Button Events
        btnFirstPageSale.addEventListener('click', function () {
            Sales.moveToFirstPage();
            UpdateNavigateButtonsSale();
        });
        btnPreviousPageSale.addEventListener('click', function () {
            Sales.moveToPreviousPage();
            UpdateNavigateButtonsSale();
        });
        btnNextPageSale.addEventListener('click', function () {
            Sales.moveToNextPage();
            UpdateNavigateButtonsSale();
        });
        btnLastPageSale.addEventListener('click', function () {
            Sales.moveToLastPage();
            UpdateNavigateButtonsSale();
        });

        $('.scroll').slimscroll({
            height: '450px'
        });
    //});
})