function calculated() {

    if (Xrm.Page.getAttribute("cr59f_carclass").getValue() != null && Xrm.Page.getAttribute("cr59f_carclass").getValue()[0].id != null) {

        var carClassId = Xrm.Page.getAttribute("cr59f_carclass").getValue()[0].id;

        var startDate = Xrm.Page.getAttribute("cr59f_actualpickup").getValue();
        var endDate = Xrm.Page.getAttribute("cr59f_actualreturn").getValue();

        if (startDate != null && endDate != null) {
            var dayCount = Math.floor((endDate - startDate) / (60 * 60 * 24 * 1000));
        } else {
            var dayCount = 1;
        }
        
        var pickupLocation = Xrm.Page.getAttribute("cr59f_pickuplocation").getValue();
        var returnLocation = Xrm.Page.getAttribute("cr59f_returnlocation").getValue();

        Xrm.WebApi.retrieveRecord("cr59f_carclass", carClassId, "?$select=cr59f_price").then(
            function success(result) {
                if (result != null) {
                    if (result.cr59f_price != null) {
                        var res = result.cr59f_price[0] * dayCount;
                        if (pickupLocation == 257700002)
                            res += 100;
                        if (returnLocation == 257700002)
                            res += 100;
                        Xrm.Page.getAttribute("cr59f_price").setValue(res);
                    }
                }
            },
            function (error) {
                console.log(error.message);
            }
        );
    }
}

var Sdk = window.Sdk || {};
(
    function () {

        this.formOnLoad = function (executionContext) {

        }
        this.dateOnChange = function (executionContext) {
            calculated();
        }
        this.locationOnChange = function (executionContext) {
            calculated();
        }
    }
).call(Sdk);