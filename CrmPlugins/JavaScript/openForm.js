function pickupButtonClick() {

    var entityFormOptions = {};
    entityFormOptions["entityName"] = "cr59f_cartransferreport";
    entityFormOptions["useQuickCreateForm"] = true;

    var car = window.parent.Xrm.Page.getAttribute("cr59f_car").getValue();
    var date = new Date(Date.now());

    var formParameters = {};
    formParameters["cr59f_type"] = 257700000;
    formParameters["cr59f_date"] = date.toISOString();

    if (car != null) {
        formParameters["cr59f_car"] = car[0].id;
        formParameters["cr59f_carname"] = car[0].name;
        formParameters["cr59f_cartype"] = "cr59f_car";
    }

    Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
        function (success) {
            Xrm.Page.getAttribute("cr59f_pickupreport").setValue(success.savedEntityReference);
        },
        function (error) {
            console.log(error);
        }
    );
}

function returnButtonClick() {

    var entityFormOptions = {};
    entityFormOptions["entityName"] = "cr59f_cartransferreport";
    entityFormOptions["useQuickCreateForm"] = true;

    var car = window.parent.Xrm.Page.getAttribute("cr59f_car").getValue();
    var date = new Date(Date.now());

    var formParameters = {};
    formParameters["cr59f_type"] = 257700001;
    formParameters["cr59f_date"] = date.toISOString();

    if (car != null) {
        formParameters["cr59f_car"] = car[0].id;
        formParameters["cr59f_carname"] = car[0].name;
        formParameters["cr59f_cartype"] = "cr59f_car";
    }

    Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
        function (success) {
            Xrm.Page.getAttribute("cr59f_returnreport").setValue(success.savedEntityReference);
        },
        function (error) {
            console.log(error);
        }
    );
}