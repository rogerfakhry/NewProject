var mywindow = undefined;

Number.prototype.formatMoney = function (c, d, t) {
    var n = this,
        c = isNaN(c = Math.abs(c)) ? 2 : c,
        d = d == undefined ? "." : d,
        t = t == undefined ? "," : t,
        s = n < 0 ? "-" : "",
        i = parseInt(n = Math.abs(+n || 0).toFixed(c)) + "",
        j = (j = i.length) > 3 ? j % 3 : 0;
    return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
};


//$.validator.addMethod('date',
//function (value, element) {
//    if (this.optional(element)) {
//        return true;
//    }
//    var valid = true;
//    try {
//        $.datepicker.parseDate('dd/mm/yy', value);
//    }
//    catch (err) {
//        valid = false;
//    }
//    return valid;
//});

$.validator.addMethod("lessThan",
function (value, element, params) {

    if ($(params).val() == "") {
        return true;
    } else {
        var svalue = value.split("/");
        var evalue = $(params).val().split("/");
        return new Date(svalue[2], svalue[1] - 1, svalue[0]) <= new Date(evalue[2], evalue[1] - 1, evalue[0]);
    }
}, 'Must be less than {0}.');


$(window).load(function () {
    $(".sub-content-container").css("min-height", $(".page-sidebar").height()-21);
});

$(function () {
        $.each($(".datepicker input"), function (index, item) {
            $(item).kendoDatePicker({ format: "dd MMMM yyyy" });
            $(item).attr("readonly", "readonly").click(function () {
                $(this).parents(".k-datepicker").find(".k-select").click();
            });
        });


    $(".menu-button").click(function () {
        $('.page-sidebar').toggleClass("show");
        $('.menu-lock').toggleClass("show");
    });

    $(".menu-lock").click(function (e) {
        if ($('.page-sidebar').hasClass("show")) {
            $('.page-sidebar').removeClass("show");
            $('.menu-lock').removeClass("show");
            e.preventDefault();
            e.stopPropagation();
        }

    });

    $(window).scroll(function (e) {
        if ($(window).scrollTop() > 180) {
            $("body").addClass("fix-search");
            $(".sub-header-buttons").css("top", $(".breadcrumb").outerHeight());
        } else {
            $("body").removeClass("fix-search");
            $(".sub-header-buttons").removeAttr("style");
        }
    });

    var trials = 0;
    var intervalId = setInterval(function () {
        $.ajax({
            url: $('#route').val() + 'shared/StayAwake',
            success: function (result) {
            }
        });

        if (trials == 25) {
            clearInterval(intervalId);
        }
    }, 210000);
    

});

var noHint = $.noop;

function SortPlaceholder(element) {
    return element.clone().addClass("k-state-hover").css("opacity", 0.65);
}

///////////////////////////////////////////////////////////////////////////
/////////////////////////Google map////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////

function LoadGoogleMapScript(callback) {
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.src = 'https://maps.googleapis.com/maps/api/js?key=AIzaSyDY51Cj9JnKK76_N92zXn6iamMnaKUdN7c&callback=' + callback;
    document.body.appendChild(script);
}

function InitializeGoogleMap(mapContainerId, latInput, latValue, lngInput, lngValue, zoomInput, zoomValue, isDragable) {
    var latlng = new google.maps.LatLng(latValue, lngValue);
    var mapOptions = {
        zoom: zoomValue,
        center: latlng
    };

    var map = new google.maps.Map(document.getElementById(mapContainerId),
        mapOptions);

    var marker = new google.maps.Marker({
        position: latlng,
        draggable: isDragable,
        animation: google.maps.Animation.DROP,
        map: map
    });

    if (isDragable) {
        google.maps.event.addListener(marker, 'dragend', function (event) {
            $('#' + latInput).val(event.latLng.lat());
            $('#' + lngInput).val(event.latLng.lng());
        });

        google.maps.event.addListener(map, 'zoom_changed', function () {
            var zoomLevel = map.getZoom();
            $('#' + zoomInput).val(map.getZoom());
        });
    }
    $(window).resize(function () {
        if ($(window).width() < 600) {
            google.maps.event.trigger(map, "resize");
        }
    });
    $(window).load(function () {
        if ($(window).width() < 600) {
            google.maps.event.trigger(map, "resize");
        }
    });
}

///////////////////////////////////////////////////////////////////////////
/////////////////////////End Google map////////////////////////////////////
///////////////////////////////////////////////////////////////////////////

function OnSortChange(e) {
    var grid = $(".grid").data("kendoGrid"),
        oldIndex = e.oldIndex,
        newIndex = e.newIndex,
        data = grid.dataSource.data(),
        pageNumber = grid.dataSource.page(),
        pageSize = grid.dataSource.pageSize(),
        totalPages = grid.dataSource.totalPages(),
        dataItem = grid.dataSource.getByUid(e.item.data("uid"));

    if (newIndex == (pageSize - 1) && pageNumber < totalPages) {
        if (confirm("Want you move this record to next page?")) {
            newIndex++;
            pageNumber = pageNumber + 1;
        }
    }

    if (newIndex == 0 && pageNumber != 1) {
        if (confirm("Want you move this record to previous page?")) {
            newIndex--;
            pageNumber = pageNumber - 1;
        }
    }

    $.ajax({
        url: $('#route').val() + $('#controller').val() + '/SortGrid',
        data: { oldIndex: oldIndex, newIndex: newIndex, id: dataItem.id },
        success: function (result) {
            grid.dataSource.page(pageNumber);
        }
    });
}

function GoToUpdatePage(obj,event, action, controller,parentId) {
    event.preventDefault();
    event.stopPropagation();
    controller = controller == undefined ? $("#controller").val() : controller;
    action = action == undefined ? "update" : action;

    var itemId = null;
    if (obj != undefined) {
        var grid = obj.parents(".grid").data("kendoGrid");
        var item = grid.dataSource._data[obj.parents("tr").index()];
        itemId = item.id;
    }
    window.location = $("#route").val() + controller + "/" + action + "/" + itemId + (parentId ==undefined ? "":"?parentId="+parentId);
}



function GoToNextPagePage(obj, event) {
    event.preventDefault();
    event.stopPropagation();

    var itemId = null;
    if (obj != undefined) {
        var grid = obj.parents(".grid").data("kendoGrid");
        var item = grid.dataSource._data[obj.parents("tr").index()];
        itemId = item.id;
    }
    window.location = $("#route").val() + $("#controller").val() + "?parentId=" + itemId;
}

function GoToAnotherPage(obj, event,url) {
    event.preventDefault();
    event.stopPropagation();

    var itemId = null;
    if (obj != undefined) {
        var grid = obj.parents(".grid").data("kendoGrid");
        var item = grid.dataSource._data[obj.parents("tr").index()];
        itemId = item.id;
    }
    window.location = url+ "?parentId=" + itemId;
}

function GoToAnotherPage1(obj, event, url) {
    event.preventDefault();
    event.stopPropagation();

    var itemId = null;
    if (obj != undefined) {
        var grid = obj.parents(".grid").data("kendoGrid");
        var item = grid.dataSource._data[obj.parents("tr").index()];
        itemId = item.id;
    }
    window.location = url + "/" + itemId;
}


function GoToReportPage(obj, event, url) {
    event.preventDefault();
    event.stopPropagation();

    var itemId = null;
    if (obj != undefined) {
        var grid = obj.parents(".grid").data("kendoGrid");
        var item = grid.dataSource._data[obj.parents("tr").index()];
        itemId = item.id;
    }
    var win = window.open(url + itemId, '_blank');
    win.focus();
}




function AddError(msg, link, confirmCallBack, cancelCallBack) {
    var errorsContainer = $('.errors-section');
    if (errorsContainer.text() == '') {
        errorsContainer.show();
    }
    if (errorsContainer.html().indexOf(msg) == -1) {
        if (link != undefined) {
            errorsContainer.append('<li>' + msg + '<a class="confirm" href="' + link + '">Yes</a> <span class="remove-error">No</span></li>');
        } else if (confirmCallBack != undefined) {
            errorsContainer.append('<li>' + msg + '<a class="confirm">Yes</a> <span class="remove-error">No</span></li>');
            $('.confirm').click(function() {
                $(this).parent().remove();
                CloseErrorsContainer();
                eval(confirmCallBack);
            });
        } else {
            errorsContainer.append('<li>' + msg + ' <span class="remove-error">Ok</span></li>');
        }
        $('.remove-error').click(function() {
            $(this).parent().remove();
            CloseErrorsContainer();
            if (cancelCallBack != undefined) {
                eval(cancelCallBack);
            }
        });
        $("html, body").animate({ scrollTop: 0 }, '500', 'swing', function () {});
    }
}

function CloseErrorsContainer() {
    var errorsContainer = $('.errors-section');
    if (errorsContainer.html() == '') {
        errorsContainer.hide();
    }
}

function UpdateHash(hash, hashValue) {
    var hashArray = window.location.hash.split('&');
    var hashData = '';
    var thisExist = false;
    $.each(hashArray, function(index, item) {
        if (item == 'null') {
            // nothing to do
        } else
        if (item.indexOf(hash) == 0 || item.indexOf("#" + hash) == 0) {
            if (hashValue != undefined) {
                hashData += (hashData == '' || hashData == 'null' ? "" : "&") + hash + "=" + hashValue;
                thisExist = true;
            }
        } else {
            hashData += (hashData == '' || hashData == 'null' ? "" : "&") + item
        }
    });
    window.location.hash = hashData == '' ? 'null' : hashData;
    if (!thisExist && hashValue != undefined) {
        if (window.location.hash == '#null') {
            window.location.hash = hash + "=" + hashValue;
        } else {
            window.location.hash += (window.location.hash == '' ? "" : "&") + hash + "=" + hashValue;
        }
    }
}

function GetHashValue(hash) {
    var hashArray = window.location.hash.split('&');
    var hashvalue = '';
    $.each(hashArray, function(index, item) {
        if (item.indexOf(hash) == 0 || item.indexOf("#" + hash) == 0) {
            var itemArray = item.split('=');
            if (itemArray.length >= 2) {
                hashvalue = itemArray[1];
            }
        }
    });
    return hashvalue;
}

function InitRepeater(id, template, values,callBAckFunction) {
    var itemsCount = $("#" + id).data("itemsCount");
    itemsCount = parseInt(itemsCount == undefined ? 0 : itemsCount);
    var result = "";
    if (values != null && values.length > 0) {
        $.each(values, function (bindex, bitem) {
            itemsCount++;
            result += '<div class="repeater-item-container"><div  class="btn delete">X</div>';
            $.each(template, function (index, item) {

                if (item.inputTag != "hidden") {
                    result += '<label class="control-label inlineblock" for="' + item.inputName + itemsCount + '">' + item.label + '</label>';
                }

                if (item.inputTag == "textarea") {
                    result += '<textarea name="' + item.inputName + itemsCount + '" id="' + item.inputName + itemsCount + '"  class="control-item inlineblock">' + bitem[index] + '</textarea>';
                } else if (item.inputTag == "select") {
                    result += '<select name="' + item.inputName + itemsCount + '" id="' + item.inputName + itemsCount + '"  class="control-item inlineblock">';
                    if (item.data) {
                        result += '<option value="">Select ' + item.label + '</option>';
                        $.each(item.data, function (dataindex, dataitem) {
                            result += '<option value="' + dataitem.id + '" ' + (bitem[index] == dataitem.id ?'selected="seleccted"':'') + '>' + dataitem.title + '</option>';
                        });
                    }
                    result += '</select>';
                } else if (item.inputTag == "info") {
                    var allSum = 0;
                    $.each(bitem[index], function (ccindex, citem) {
                        allSum += parseFloat(citem[2]);
                    });

                    result += "<div class='repeater-info' style='" + (parseFloat(bitem[2]) - allSum <0 ?"color:red;font-weight:bold;":"") + "'>Rest: " + (parseFloat(bitem[2]) - allSum) + "</div>";
                    $.each(bitem[index], function (ccindex, citem) {
                        result += "<div class='repeater-info'>Claim: <a  target='_blank' href='" + $("#route").val() + "claim/update/" + citem[0] + "' >" + citem[1]  + "</a>  " + citem[4] + "<br/>Amount: " + citem[2] + "<br/>Remarks: " + citem[3] + "</div>";
                    });
                } else {
                    result += '<input name="' + item.inputName + itemsCount + '" id="' + item.inputName + itemsCount + '"  class="control-item inlineblock" value=' + bitem[index] + ' type="' + item.inputTag + '"/>';
                }
            });
            result += "</div></div>";
        });
    } else {
        itemsCount++;
        result += '<div class="repeater-item-container"><div  class="btn delete">X</div>';
        $.each(template, function (index, item) {
            if (item.inputTag != "hidden") {
                result += '<label class="control-label inlineblock" for="' + item.inputName + itemsCount + '">' + item.label + '</label>';
            }
            if (item.inputTag == "textarea") {
                result += '<textarea name="' + item.inputName + itemsCount + '" id="' + item.inputName + itemsCount + '"  class="control-item inlineblock"></textarea>';
            } else if (item.inputTag == "select") {
                result += '<select name="' + item.inputName + itemsCount + '" id="' + item.inputName + itemsCount + '"  class="control-item inlineblock">';
                if(item.data){
                    result += '<option value="">Select ' + item.label + '</option>';
                    $.each(item.data, function (dataindex, dataitem) {
                        result += '<option value="' + dataitem.id + '">' + dataitem.title+ '</option>';
                    });
                }
                result += '</select>';
            } else if (item.inputTag == "info") {

            }
            else if (item.inputTag == "hidden"){
                result += '<input name="' + item.inputName + itemsCount + '" id="' + item.inputName + itemsCount + '"  class="control-item inlineblock" value="-1" type="' + item.inputTag + '"/>';
            }
            else {
                result += '<input name="' + item.inputName + itemsCount + '" id="' + item.inputName + itemsCount + '"  class="control-item inlineblock" type="' + item.inputTag + '"/>';
            }
        });
        result += "</div></div>";
    }

    $("#" + id).data("itemsCount", itemsCount);
    $("#" + id + " .repeater-inner-container").append(result);
    $("#" + id + " .delete").unbind("click");
    $("#" + id + " .delete").bind("click", function () {
        $(this).parents(".repeater-item-container").remove();
    });
    if (callBAckFunction != undefined) {
        callBAckFunction();
    }
}

function AddDEductionDetails(id, providerContractTariffId, hospitalClassId,isHospital,claimId) {
    var itemsCount = $("#" + id).data("itemsCount");
    itemsCount = parseInt(itemsCount == undefined ? 0 : itemsCount);
    claimId = claimId == undefined ? "" : claimId;

    //$(".send-email .loader").show();
    //$(".send-email").prop("disabled", true);
    //$(".notification-container").html("");
    $.ajax({
        url: $('#route').val() + 'claim/deductionformitem' + (isHospital == true ?"?ishospital=hos-":""),
        data: { nextindex: itemsCount + 1, providerContractTariffId: providerContractTariffId, hospitalClassId: hospitalClassId, claimId: claimId },
        method: "GET",
        success: function (result) {
            $("#" + id).data("itemsCount", itemsCount+1);
            $("#" + id + " .repeater-inner-container").append(result);
            $("#" + id + " .delete").unbind("click");
            $("#" + id + " .delete").bind("click", function () {
                $(this).parents(".repeater-item-container").remove();
            });

            InitOnlyNumber();
        }
    });
}
function GetCodes(index, itemId, providerContractTariffId, hospitalClassId, isHospital) {
    if (itemId == undefined) {
        $("#" + isHospital + "diagnosticListShareTypeId" + index).html("<option rcoefficient='0' acoefficient='0' price='0' rate='0' discount='0' vlaue=''>Select Code</option>");
        GetTypeData(index, isHospital);
    }else{
       // $(".send-email .loader").show();
       // $(".send-email").prop("disabled", true);
       // $(".notification-container").html("");
        $.ajax({
            url: $('#route').val() + 'contractservice/GetCodes',
            data: { requestedItemId: $("#" + isHospital + "requestedDiagnosticListId" + index).val(), approvedItemId: itemId, providerContractTariffId: providerContractTariffId, hospitalClassId: hospitalClassId },
            method: "GET",
            dataType: "json",
            success: function (data) {
                if (data.status == "success") {
                    var result = "<option rcoefficient='0' acoefficient='0' price='0' rate='0' discount='0' vlaue=''>Select Code</option>";
                    $.each(data.codes, function (index, item) {
                        var ritemCoefficient = 0;
                        var aitemCoefficient = 0;
                        $.each(data.rcoefficients, function (subIndex, subItem) {
                            if (subItem.id == item.id) {
                                ritemCoefficient = subItem.coefficient;
                            }
                        });
                        $.each(data.acoefficients, function (subIndex, subItem) {
                            if (subItem.id == item.id) {
                                aitemCoefficient = subItem.coefficient;
                            }
                        });
                        result += "<option nssfprice='" + item.nssfprice + "' nssfdiscount='" + item.nssfdiscount + "' rcoefficient='" + ritemCoefficient + "' acoefficient='" + aitemCoefficient + "' price='" + item.price + "' rate='" + item.currencyRate + "' nssfrate='" + item.nssfCurrencyRate + "' discount='" + item.discount + "' value='" + item.id + "'>" + item.type + " c:" + item.currency + " nssfcr:" + item.nssfCurrencyRate + "</option>";
                    });
                    $("#" + isHospital + "diagnosticListShareTypeId" + index).html(result);
                } else {
                    $("#" + isHospital + "diagnosticListShareTypeId" + index).html("<option rcoefficient='0' acoefficient='0' price='0' rate='0' nssfrate='0' discount='0' vlaue=''>Select Code</option>");
                }
                GetTypeData(index, isHospital);
            }
        });
    }
}

function UpdateTotalAmount() {
    var sum = 0;
    $('input[id*="payableAmount"]').each(function () {
        sum += parseFloat($(this).val()); 
    });
    $(".total-amount").text(sum.formatMoney(2, '.', ','));
}

function UpdateRequestedAmount() {
    var sum = 0;
    $('input[id*="requestedAmount"]').each(function () {
        sum += parseFloat($(this).val());
    });
    $(".requested-amount").text(sum.formatMoney(2, '.', ','));
}




function GetTypeData(index, isHospital) {
    setTimeout(function () {
        var obj = $("#" + isHospital + "diagnosticListShareTypeId" + index + " option:selected");
        $("#" + isHospital + "approvedCoefficient" + index).val(obj.attr("acoefficient"));
        $("#" + isHospital + "requestedCoefficient" + index).val(obj.attr("rcoefficient"));
        $("#" + isHospital + "typeprice" + index).val(obj.attr("price"));
        $("#" + isHospital + "typediscount" + index).val(obj.attr("discount"));
        $("#" + isHospital + "typecurrencyRate" + index).val(obj.attr("rate"));

        var realCoefficient = parseFloat($("#" + isHospital + "realCoefficient" + index).val());
        var requestedCoefficient = parseFloat($("#" + isHospital + "requestedCoefficient" + index).val());
        var approvedCoefficient = parseFloat($("#" + isHospital + "approvedCoefficient" + index).val());
        var typeprice = parseFloat($("#" + isHospital + "typeprice" + index).val());
        var nssfprice = $("#hasNssf").val() == "True" ? parseFloat(obj.attr("nssfprice")) : 0;
        var nssfcurrencyRate = parseFloat(obj.attr("nssfrate"));
        var typediscount = parseFloat($("#" + isHospital + "typediscount" + index).val());
        var typecurrencyRate = parseFloat($("#" + isHospital + "typecurrencyRate" + index).val());

        var nssfprice = (realCoefficient * approvedCoefficient * nssfprice) / nssfcurrencyRate;
        var finalnssfprice = Math.round((nssfprice * parseFloat($("#nssfSharePercent").val()) / 100) * 100) / 100;

        var rprice = realCoefficient * requestedCoefficient * typeprice;
        var aprice = realCoefficient * approvedCoefficient * typeprice;

        $("#" + isHospital + "discountAmount" + index).val(aprice * typediscount / 100)
        var discountAmount = parseFloat($("#" + isHospital + "discountAmount" + index).val());

        $("#" + isHospital + "nssfShareAmount" + index).val(finalnssfprice);

        //$("#" + isHospital + "requestedAmount" + index).val(Math.round((rprice - (rprice * typediscount / 100)) * 100) / 100);
        //$("#" + isHospital + "approvedAmount" + index).val(Math.round((aprice - (aprice * typediscount / 100)) * 100) / 100);
        //$("#" + isHospital + "deductionAmount" + index).val(Math.round((rprice - (rprice * typediscount / 100)) - (aprice - (aprice * typediscount / 100))));

        //$("#" + isHospital + "payableAmount" + index).val(Math.round((aprice - (aprice * typediscount / 100) - discountAmount - finalnssfprice) * 100) / 100);

        $("#" + isHospital + "requestedAmount" + index).val(Math.round(rprice * 100) / 100);
        $("#" + isHospital + "approvedAmount" + index).val(Math.round(aprice * 100) / 100);
        $("#" + isHospital + "deductionAmount" + index).val(Math.round((rprice - aprice) * 100) / 100);

        $("#" + isHospital + "payableAmount" + index).val(Math.round((aprice - discountAmount - finalnssfprice) * 100) / 100);
        UpdateTotalAmount();
        UpdateRequestedAmount();
    }, 200);
}

function UpdatededucDetaislData(event, index, objName, isHospital) {


    var typeprice = parseFloat($("#" + isHospital + "typeprice" + index).val());
    var typediscount = parseFloat($("#" + isHospital + "typediscount" + index).val());
    var typecurrencyRate = parseFloat($("#" + isHospital + "typecurrencyRate" + index).val());

    if (objName == "realCoefficient" || objName == "requestedCoefficient" || objName == "approvedCoefficient")
    {
        UpdateApprovedAmount(isHospital, objName);
    }


    if (objName == "requestedAmount" || objName == "approvedAmount" || objName == "nssfShareAmount") {
        UpdateDeductionAmount(isHospital);
    }


    if (objName == "discountAmount") {
        UpdatePayableAmount(isHospital);
    }

    function UpdateApprovedAmount(isHospital, coefficient) {
        var requestedCoefficient = parseFloat($("#" + isHospital + "requestedCoefficient" + index).val());
        var approvedCoefficient = parseFloat($("#" + isHospital + "approvedCoefficient" + index).val());

        var realCoefficient = parseFloat($("#" + isHospital + "realCoefficient" + index).val());
        var rprice = realCoefficient * requestedCoefficient * typeprice;
        var aprice = realCoefficient * approvedCoefficient * typeprice;

        $("#" + isHospital + "discountAmount" + index).val(aprice * typediscount / 100)
        var discountAmount = parseFloat($("#" + isHospital + "discountAmount" + index).val());


        //if (coefficient == "realCoefficient" || coefficient == "requestedCoefficient") {
        //    $("#" + isHospital + "requestedAmount" + index).val(Math.round((rprice - (rprice * typediscount / 100)) * 100) / 100);
        //}
        //if (coefficient == "realCoefficient" || coefficient == "approvedCoefficient") {
        //    $("#" + isHospital + "approvedAmount" + index).val(Math.round((aprice - (aprice * typediscount / 100)) * 100) / 100);
        //}

        if (coefficient == "realCoefficient" || coefficient == "requestedCoefficient") {
            $("#" + isHospital + "requestedAmount" + index).val(Math.round((rprice) * 100) / 100);
        }
        if (coefficient == "realCoefficient" || coefficient == "approvedCoefficient") {
            $("#" + isHospital + "approvedAmount" + index).val(Math.round((aprice) * 100) / 100);
        }

        //#region nssf price
            var obj = $("#" + isHospital + "diagnosticListShareTypeId" + index + " option:selected");
            var nssfprice = $("#hasNssf").val() == "True" ? parseFloat(obj.attr("nssfprice")) : 0;
            var nssfcurrencyRate = parseFloat(obj.attr("nssfrate"));
            var nssfprice = nssfcurrencyRate == 0 ? 0 : (realCoefficient * approvedCoefficient * nssfprice) / nssfcurrencyRate;
        //var nssfprice = realCoefficient * parseFloat(obj.attr("acoefficient")) * nssfprice;
            var finalnssfprice = Math.round((nssfprice * parseFloat($("#nssfSharePercent").val()) / 100) * 100) / 100;
            $("#" + isHospital + "nssfShareAmount" + index).val(finalnssfprice);
        //#endregion

        UpdateDeductionAmount(isHospital)
    }

    function UpdateDeductionAmount(isHospital) {
        var requestedAmount = parseFloat($("#" + isHospital + "requestedAmount" + index).val());
        var approvedAmount = parseFloat($("#" + isHospital + "approvedAmount" + index).val());
        $("#" + isHospital + "deductionAmount" + index).val(Math.round((requestedAmount - approvedAmount)*100)/100);
        UpdatePayableAmount(isHospital);
    }

    function UpdatePayableAmount(isHospital) {
        var realCoefficient = parseFloat($("#" + isHospital + "realCoefficient" + index).val());
        var requestedCoefficient = parseFloat($("#" + isHospital + "requestedCoefficient" + index).val());
        var approvedCoefficient = parseFloat($("#" + isHospital + "approvedCoefficient" + index).val());
        var approvedAmount = parseFloat($("#" + isHospital + "approvedAmount" + index).val());
        var discountAmount = parseFloat($("#" + isHospital + "discountAmount" + index).val());

        //#region nssf price
        var obj = $("#" + isHospital + "diagnosticListShareTypeId" + index + " option:selected");
        var nssfprice = $("#hasNssf").val() == "True" ? parseFloat(obj.attr("nssfprice")) : 0;
        var nssfcurrencyRate = parseFloat(obj.attr("nssfrate"));
        var nssfprice = nssfcurrencyRate == 0 ? 0 : (realCoefficient * approvedCoefficient * nssfprice) / nssfcurrencyRate;
        //var nssfprice = realCoefficient * parseFloat(obj.attr("acoefficient")) * nssfprice;
        var finalnssfprice = (nssfprice * parseFloat($("#nssfSharePercent").val()) / 100);

        
        $("#" + isHospital + "payableAmount" + index).val(Math.round((approvedAmount - discountAmount - finalnssfprice) * 100) / 100);
        UpdateTotalAmount();
        UpdateRequestedAmount();
    }
}

function InitOnlyNumber() {
    $(".onlyNumber").keydown(function (event) {
        // Allow: backspace, delete, tab, escape, dot, and enter
        if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 27 || event.keyCode == 110 || event.keyCode == 13 ||
            // Allow: Ctrl+A
            (event.keyCode == 65 && event.ctrlKey === true) ||
            // Allow: F5
            (event.keyCode == 116) ||
            // Allow: -
            ((event.keyCode == 109 || event.keyCode == 189)) ||
            // Allow: home, end, left, right
            (event.keyCode >= 35 && event.keyCode <= 39)) {
            // let it happen, don't do anything
            return;
        }
        else {
            // Ensure that it is a number and stop the keypress
            if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                event.preventDefault();
            }
        }
    })
}
////////////////////////////////////////////
///////////Export Grid To Execel////////////
///////////////////////////////////////////
var convertObjectToQueryString = function (inputObject) {
    var str = '';

    for (var key in inputObject) {
        if (!inputObject.hasOwnProperty(key) || typeof inputObject[key] === 'function') { continue; }
        if (Array.isArray(inputObject[key])) {
            for (var i = 0; i < inputObject[key].length; i++) {
                str += key + '['+i+']=' + encodeURIComponent(inputObject[key][i]) + '&';
            }
        }else
        if (typeof inputObject[key] === 'object') {
            str += convertObjectToQueryString(inputObject[key]);
        } else {
            str += key + '=' + encodeURIComponent(inputObject[key]) + '&';
        }
    }

    return str;
};
function PrepareDataFoExportExcel(addData) {
    var grid = $('#grid').data('kendoGrid');
    var $exportLink = $('#exportcsv');
    var href = $exportLink.attr('href'); 
    
    if (addData != undefined) {  
        href = $exportLink.attr('defaulthref'); 
    }
    if (href != undefined) {

        var requestObject = (new kendo.data.transports["aspnetmvc-server"]({ prefix: "" }))
             .options.parameterMap({
                 //page: grid.dataSource.page(),
                 sort: grid.dataSource.sort(),
                 filter: grid.dataSource.filter()
             });
        href = href.replace(/sort=([^&]*)/, 'sort=' + requestObject.sort || '~');
        href = href.replace(/filter=([^&]*)/, 'filter=' + (requestObject.filter || '~'));
        if (addData != undefined) {
            var additionalData = eval(addData);
            href = href + '&' + decodeURIComponent(convertObjectToQueryString(additionalData));
        }
        $exportLink.attr('href', href);
    }
}
function PrepareDataFoPrint(addData) {
    var grid = $('#grid').data('kendoGrid');
    var $exportLink = $('#print');
    var href = $exportLink.attr('href');

    if (addData != undefined) {
        href = $exportLink.attr('defaulthref');
    }
    if (href != undefined) {

        var requestObject = (new kendo.data.transports["aspnetmvc-server"]({ prefix: "" }))
             .options.parameterMap({
                 //page: grid.dataSource.page(),
                 sort: grid.dataSource.sort(),
                 filter: grid.dataSource.filter()
             });
        href = href.replace(/sort=([^&]*)/, 'sort=' + requestObject.sort || '~');
        href = href.replace(/filter=([^&]*)/, 'filter=' + (requestObject.filter || '~'));
        if (addData != undefined) {
            var additionalData = eval(addData);
            href = href + '&' + decodeURIComponent(convertObjectToQueryString(additionalData));
        }
        $exportLink.attr('href', href);
    }
}
////////////////////////////////////////////
/////////End Export Grid To Execel//////////
///////////////////////////////////////////


function PrintData(data) {
    if (mywindow != undefined) {
        mywindow.close();
    }
    mywindow = window.open('', 'Print', 'height=800px,width=1024px');
    mywindow.document.write('<html><head><title>Print</title>');
    /*optional stylesheet*/ //mywindow.document.write('<link rel="stylesheet" href="main.css" type="text/css" />');
    mywindow.document.write('</head><body >');
    mywindow.document.write(data);
    mywindow.document.write('</body></html>');

    mywindow.document.close(); // necessary for IE >= 10
    mywindow.focus(); // necessary for IE >= 10

    mywindow.print();
    mywindow.close();

    return true;
}
      

function PrintData1(obj, pageHeigth) {
    if (mywindow != undefined) {
        mywindow.close();
    }
    mywindow = window.open('', 'Print', 'height=800px,width=1024px');
    mywindow.document.write('<html><head><title>Print</title><style type="text/css">table { page-break-inside:avoid;page-break-after:auto}tr{ page-break-inside:avoid; }</style>');
    /*optional stylesheet*/ //mywindow.document.write('<link rel="stylesheet" href="main.css" type="text/css" />');
    mywindow.document.write('</head><body style="margin:0;"><table>');
    obj.attr("style", "position:absolute;left:-100000px;bottom:-10000px;display:block;");
    var header = obj.find('> table > thead');
    var currentPageHeigth = 0;

    $.each(header.find('> tr'), function (subindex, subitem) {
        mywindow.document.write($(subitem).prop('outerHTML'));
        currentPageHeigth += $(subitem).outerHeight();
    });
   // mywindow.document.write("</thead>");

    $.each(obj.find(' > table > tbody > tr'), function (index, item) {
        if (currentPageHeigth + $(item).height() > pageHeigth) {
            mywindow.document.write('</table><table>');
            currentPageHeigth = 0;

            $.each(header.find('> tr.header'), function (subindex, subitem) {
                mywindow.document.write($(subitem).prop('outerHTML'));
                currentPageHeigth += $(subitem).outerHeight();
            });
           // mywindow.document.write("</thead>");

            mywindow.document.write($(item).prop('outerHTML'));
            currentPageHeigth += $(item).outerHeight();
        } else {
            mywindow.document.write($(item).prop('outerHTML'));
            currentPageHeigth += $(item).outerHeight();
        }
    });

    mywindow.document.write('</table></body></html>');
    obj.removeAttr("style");
    mywindow.document.close(); // necessary for IE >= 10
    mywindow.focus(); // necessary for IE >= 10

    mywindow.print();
   mywindow.close();

    return true;
}
function SendEmail(obj,data, email, subject) {
    obj.find(".loader").show();
    obj.prop("disabled",true);
    $(".notification-container").html("");
    $.ajax({
        url: $('#route').val() + 'shared/SendEmail',
        data: { data: data.replace("Printed", "Sent"), email: email, subject: subject },
        method:"POST",
        dataType: "json",
        success: function (data) {
            if (data.status == "success") {
                $(".notification-container").html("An email has been sent to " + email);
            } else {
                $(".notification-container").html("Something went wrong please retry.");
            }
            obj.prop("disabled", false);
            obj.find(".loader").hide();
        }
    });
}

function DisableAll() {
    $("input,textarea").attr("readonly", true);
    $("select,input[type='checkbox']").attr("disabled", true);
    $(".delete").remove();
    $(".add-item").remove();
    $.each($('.k-datepicker input'), function (index, item) {
        $(item).data('kendoDatePicker').enable(false);
    });
    $.each($('.k-editor-widget textarea'), function (index, item) {
            $($(item).data().kendoEditor.body).attr('contenteditable', false);
    });

    $(".fileinput-button").remove();
}

function SaveHistory(e,additionalParam) {
    var grid = $('#grid').data('kendoGrid');
    // ask the parameterMap to create the request object for you
    var requestObject = (new kendo.data.transports["aspnetmvc-server"]({ prefix: "" }))
                        .options.parameterMap({
                            page: grid.dataSource.page(),
                            sort: grid.dataSource.sort(),
                            filter: grid.dataSource.filter(),
                            group: grid.dataSource.group()
                        });
    history.pushState("", document.title, window.location.pathname + "?page=" + requestObject.page + "&sort=" + requestObject.sort + "&filter=" + requestObject.filter + "&group=" + requestObject.group + (additionalParam == undefined ? "" : "&" + additionalParam + "=" + $("#" + additionalParam).val()));
}


function InitPhoneFormat(obj,callBackFunction) {
    obj.keydown(function (event) {
        // Allow: backspace, delete, tab, escape, space, and enter
        if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 27 || event.keyCode == 32 || event.keyCode == 13 ||
            // Allow: Ctrl+A
            (event.keyCode == 65 && event.ctrlKey === true) ||
            // Allow: F5
            (event.keyCode == 116) ||
            (event.keyCode == 110) ||
            (event.keyCode == 190) ||
            // Allow: home, end, left, right
            (event.keyCode >= 35 && event.keyCode <= 39)) {
            // let it happen, don't do anything
            return;
        }
        else {
            // Ensure that it is a number and stop the keypress
            if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                event.preventDefault();
            }
        }
    });

    obj.keyup(function (event) {
        if (callBackFunction != undefined) {
            callBackFunction();
        }
    });
}




function SearchClaims(e) {
    var val = $("#grid #global-search").val();
    var filter = [
            {
                logic: "and",
                filters: []
            }];
    var innerFilter = {
        logic: "or",
        filters: []
    };

    if ($("#grid #global-search").val() != '') {
        innerFilter.filters.push({ field: "ClaimId", operator: "contains", value: val });
        innerFilter.filters.push({ field: "policyContainer", operator: "contains", value: val });
        innerFilter.filters.push({ field: "PatientName", operator: "contains", value: val });
        innerFilter.filters.push({ field: "ProviderName", operator: "contains", value: val });
        innerFilter.filters.push({ field: "CompanyName", operator: "contains", value: val });
        innerFilter.filters.push({ field: "status", operator: "contains", value: val });
		if($('#controller').val() == 'reportpaidclaims' ){
        innerFilter.filters.push({ field: "GetDiagnosisGrid", operator: "contains", value: val });
        innerFilter.filters.push({ field: "doctorName", operator: "contains", value: val });
        innerFilter.filters.push({ field: "invoiceNumber", operator: "contains", value: val });
		}
        filter[0].filters.push(innerFilter);
    }

    if ($("#grid #RelatedLists").val() != null && $("#grid #RelatedLists").val() != undefined) {

        $.each($("#grid #RelatedLists").val(), function (i, v) {
            innerFilter.filters.push({ field: "RelatedListsIds", operator: "contains", value: "," + v + "," });
        });
        filter[0].filters.push(innerFilter);
    }

    $("#grid").data("kendoGrid").dataSource.filter(filter[0].filters.length > 0 ? filter : null);
}
function SearchGuaranteed(e) {
    var val = $("#grid #global-search").val();
    var filter = [
            {
                logic: "and",
                filters: []
            }];
    var innerFilter = {
        logic: "or",
        filters: []
    };

    if ($("#grid #global-search").val() != '') {
        innerFilter.filters.push({ field: "FullName", operator: "contains", value: val });
        filter[0].filters.push(innerFilter);
    }
    $("#grid").data("kendoGrid").dataSource.filter(filter[0].filters.length > 0 ? filter : null);
}

function SearchPolicies(e) {
    var val = $("#grid #global-search").val();
    var filter = [
            {
                logic: "and",
                filters: []
            }];
    var innerFilter = {
        logic: "or",
        filters: []
    };

    if ($("#grid #global-search").val() != '') {
        innerFilter.filters.push({ field: "policyId", operator: "contains", value: val });
        innerFilter.filters.push({ field: "CompanyName", operator: "contains", value: val });
        innerFilter.filters.push({ field: "code", operator: "contains", value: val });
        innerFilter.filters.push({ field: "FullName", operator: "contains", value: val });
        filter[0].filters.push(innerFilter);
    }
    $("#grid").data("kendoGrid").dataSource.filter(filter[0].filters.length > 0 ? filter : null);
}

function SearchDiagnosticList(e) {
    var val = $("#grid #global-search").val();
    var filter = [
            {
                logic: "and",
                filters: []
            }];
    var innerFilter = {
        logic: "or",
        filters: []
    };

    if ($("#grid #global-search").val() != '') {
        innerFilter.filters.push({ field: "code", operator: "contains", value: val });
        innerFilter.filters.push({ field: "description", operator: "contains", value: val });
        filter[0].filters.push(innerFilter);
    }
    $("#grid").data("kendoGrid").dataSource.filter(filter[0].filters.length > 0 ? filter : null);
}

function SearchDiagnosticListShareType(e) {
    var val = $("#grid #global-search").val();
    var filter = [
            {
                logic: "and",
                filters: []
            }];
    var innerFilter = {
        logic: "and",
        filters: []
    };

    if ($("#grid #global-search").val() != '') {
        innerFilter.filters.push({ field: "title", operator: "contains", value: val });
        filter[0].filters.push(innerFilter);
    }

    if ($("#grid #RelatedClasses").val() != null && $("#grid #RelatedClasses").val() != undefined) {
        $.each($("#grid #RelatedClasses").val(), function (i, v) {
            innerFilter.filters.push({ field: "RelatedClassesIds", operator: "contains", value: "," + v + "," });
        });
        filter[0].filters.push(innerFilter);
    }

    $("#grid").data("kendoGrid").dataSource.filter(filter[0].filters.length > 0 ? filter : null);
}

function SearchCompanyContract(e) {
    var val = $("#grid #global-search").val();
    var filter = [
            {
                logic: "and",
                filters: []
            }];
    var innerFilter = {
        logic: "or",
        filters: []
    };

    if ($("#grid #global-search").val() != '') {
        innerFilter.filters.push({ field: "Deal", operator: "contains", value: val });
        innerFilter.filters.push({ field: "name", operator: "contains", value: val });
        innerFilter.filters.push({ field: "localAddress", operator: "contains", value: val });
        innerFilter.filters.push({ field: "phone", operator: "contains", value: val });
        filter[0].filters.push(innerFilter);
    }
    $("#grid").data("kendoGrid").dataSource.filter(filter[0].filters.length > 0 ? filter : null);
}


function SearchProviderContract(e) {
    var val = $("#grid #global-search").val();
    var filter = [
            {
                logic: "and",
                filters: []
            }];
    var innerFilter = {
        logic: "or",
        filters: []
    };

    if ($("#grid #global-search").val() != '') {
        innerFilter.filters.push({ field: "Deal", operator: "contains", value: val });
        innerFilter.filters.push({ field: "name", operator: "contains", value: val });
        innerFilter.filters.push({ field: "localAddress", operator: "contains", value: val });
        innerFilter.filters.push({ field: "phone", operator: "contains", value: val });
        filter[0].filters.push(innerFilter);
    }
    $("#grid").data("kendoGrid").dataSource.filter(filter[0].filters.length > 0 ? filter : null);
}


function SearchContractService(e) {
    var val = $("#grid #global-search").val();
    var filter = [
            {
                logic: "and",
                filters: []
            }];
    var innerFilter = {
        logic: "or",
        filters: []
    };

    if ($("#grid #global-search").val() != '') {
        innerFilter.filters.push({ field: "prices", operator: "contains", value: val });
        innerFilter.filters.push({ field: "DiagnosticTypeTitle", operator: "contains", value: val });
        filter[0].filters.push(innerFilter);
    }
    $("#grid").data("kendoGrid").dataSource.filter(filter[0].filters.length > 0 ? filter : null);
}


function SaveReport() {
    var title = prompt("Please enter report's title", $(".page-title").html());

    if (title != null) {
        var link = document.location.href;
        var grid = $("#grid").data("kendoGrid");
        $.ajax({
            url: $('#route').val() + 'report/create',
            data: { title: title, link: link, options: kendo.stringify(grid.columns) },
            method: "POST",
            success: function (result) {
                alert("Report was successfully saved");
            }
        });
    }
}

function LoadReport(obj, event) {
    event.preventDefault();
    event.stopPropagation();

    var itemId = null;
    var link = null;
    if (obj != undefined) {
        var grid = obj.parents(".grid").data("kendoGrid");
        var item = grid.dataSource._data[obj.parents("tr").index()];
        itemId = item.id;
        link = item.link;
    }
    window.open(link + "&reportId=" + itemId, '_blank');
}

function setColumnWidths(grid, columns) {
    var lockedCount = 0;
    for (var i = 0; i < columns.length; i++) {
        if (columns[i].hasOwnProperty('locked')) {
            if (columns[i].locked) {
                lockedCount++;
            }
        }
    }

    for (var i = 0; i < columns.length; i++) {
        var width = columns[i].width;
        grid.columns[i].width = width;
        if (columns[i].hasOwnProperty('locked') && columns[i].locked) {
            $("#grid .k-grid-header-locked").find("colgroup col").eq(i).width(width);
            $("#grid .k-grid-content-locked").find("colgroup col").eq(i).width(width);

        } else {
            $("#grid .k-grid-header-wrap").find("colgroup col").eq(i - lockedCount).width(width);
            $("#grid .k-grid-content").find("colgroup col").eq(i - lockedCount).width(width);
        }
    }
    // Hack to refresh grid visual state
    grid.reorderColumn(1, grid.columns[0]);
    grid.reorderColumn(1, grid.columns[0]);
}

function LoadReportOptions(options) {
    var grid = $("#grid").data("kendoGrid");
    var options = JSON.parse(options);
    setTimeout(function () { 
    $.each(options, function (index, item) {
        if (item.hidden != undefined) {
            if (item.hidden == true) {
                grid.hideColumn(index);
            } else {
                grid.showColumn(index);
            }
        }
    });
    setColumnWidths(grid, options);
    }, 1000);
}



//function getPDF(selector) {
//    kendo.drawing.drawDOM($(selector)).then(function (group) {
//        kendo.drawing.pdf.saveAs(group, "Invoice.pdf");
//    });
//}