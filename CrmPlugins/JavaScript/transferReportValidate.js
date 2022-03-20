var Sdk = window.Sdk || {};
(
    function () {

        this.formOnLoad = function (executionContext) {

            var damages = Xrm.Page.getAttribute("cr59f_damages").getValue();

            if (damages)
                Xrm.Page.getAttribute("cr59f_damagedescription").setRequiredLevel("required");
            else
                Xrm.Page.getAttribute("cr59f_damagedescription").setRequiredLevel("none");
        }

        this.damagesOnChange = function (executionContext) {

            var damages = Xrm.Page.getAttribute("cr59f_damages").getValue();

            if (damages)
                Xrm.Page.getAttribute("cr59f_damagedescription").setRequiredLevel("required");
            else
                Xrm.Page.getAttribute("cr59f_damagedescription").setRequiredLevel("none");
        }
    }
).call(Sdk);