function addCarFilter() {
    try {
        var recordId = Xrm.Page.data.entity.getId();
        var carClass = Xrm.Page.getAttribute("cr59f_carclass").getValue();

        if (carClass != null && carClass != undefined) {

            fetchXml = "<filter type='and'><condition attribute='cr59f_carclass' operator='like' uiname='" + carClass[0].name + "' uitype='cr59f_carclass' value='" + carClass[0].id + "' /></filter>";
            Xrm.Page.getControl("cr59f_car").addCustomFilter(fetchXml);
        }
    } catch (e) {
        throw new Error(e.Message);
    }
}

function filterLoockup() {
    try {

        if (Xrm.Page.getControl("cr59f_carclass") != null && Xrm.Page.getControl("cr59f_carclass") != undefined) {
            Xrm.Page.getControl("cr59f_car").addPreSearch(function () {
                addCarFilter();
            });
        }
    } catch(e) {
        throw new Error(e.Message);
    }
}

var Sdk = window.Sdk || {};
(
    function () {

        this.formOnLoad = function (executionContext) {

            var carClass = Xrm.Page.getAttribute("cr59f_carclass").getValue();

            if (carClass != null && carClass != undefined) {
                Xrm.Page.getControl("cr59f_car").setDisabled(false);
            } else {
                Xrm.Page.getControl("cr59f_car").setDisabled(true);
            }
        }

        this.carClassOnChange = function (executionContext) {
            filterLoockup();
        }
    }
).call(Sdk);