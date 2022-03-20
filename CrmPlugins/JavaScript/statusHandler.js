var createdStatus = { value: 257700000, text: "Created" };
var confirmedStatus = { value: 257700001, text: "Confirmed" };
var rentingStatus = { value: 257700002, text: "Renting" };
var returnedStatus = { value: 257700003, text: "Returned" };
var canceledStatus = { value: 257700004, text: "Canceled" };

var options = Xrm.Page.getControl("statuscode").getOptions();

function resetOptionSet() {

    for (var i = 0; i < options.length; i++) {
        Xrm.Page.getControl("statuscode").removeOption(options[i].value);
    }
}

var Sdk = window.Sdk || {};
(
    function () {

        this.formOnLoad = function (executionContext) {

            var opt = Xrm.Page.getAttribute("statuscode").getSelectedOption();

            if (opt.value == confirmedStatus.value || opt.value == rentingStatus.value || opt.value == returnedStatus.value) {
                Xrm.Page.getAttribute("cr59f_car").setRequiredLevel("required");
            } else {
                Xrm.Page.getAttribute("cr59f_car").setRequiredLevel("none");
            }
        }

        this.statusOnChange = function (executionContext) {

            var opt = Xrm.Page.getAttribute("statuscode").getSelectedOption();
            Xrm.Page.getAttribute("cr59f_car").setRequiredLevel("none");

            if (opt.value == createdStatus.value) {
                resetOptionSet();
                Xrm.Page.getControl("statuscode").addOption(createdStatus)
                Xrm.Page.getControl("statuscode").addOption(confirmedStatus)
                Xrm.Page.getControl("statuscode").addOption(rentingStatus)
                Xrm.Page.getControl("statuscode").addOption(canceledStatus)
            }
            if (opt.value == confirmedStatus.value) {
                resetOptionSet();
                Xrm.Page.getControl("statuscode").addOption(confirmedStatus)
                Xrm.Page.getControl("statuscode").addOption(rentingStatus)
                Xrm.Page.getControl("statuscode").addOption(canceledStatus)
                Xrm.Page.getAttribute("cr59f_car").setRequiredLevel("required");
            }
            if (opt.value == rentingStatus.value) {
                resetOptionSet();
                Xrm.Page.getControl("statuscode").addOption(rentingStatus)
                Xrm.Page.getControl("statuscode").addOption(returnedStatus)
                Xrm.Page.getAttribute("cr59f_car").setRequiredLevel("required");
            }
            if (opt.value == returnedStatus.value) {
                Xrm.Page.getAttribute("cr59f_car").setRequiredLevel("required");
            }
        }
    }
).call(Sdk);