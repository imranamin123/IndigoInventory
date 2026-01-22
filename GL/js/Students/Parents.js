
$().ready(function () {

    prtPageLoad();

    $(".saveStudentParents").click(function (e) {
        e.preventDefault;
        
        var StudentID = $("#StudentID").val();
        if (StudentID == '' || StudentID == undefined || StudentID == null) {
            alert("Please add the student then add parents");
            return;
        }
        
        if (!prtIsValidParent()) {
            return;
        }
        var obj = prtprtGetParentDataObject();
        var jsonData = JSON.stringify({ "studentID": $("#hdStudentID").val(), "parents": obj });

        //$.ajax({
        //    url: "/Students/StudentParentsSave",
        //    method: "post",
        //    success: function (jsonData) {
        //        
        //        alert('');
        //        //console.log(data);
        //        //$('#rolename', 'div').html('');
        //        //$(data).each(function (i, role) {
        //        //    var rolename = "<div class='checkbox-nice'><input id='UserRoles[" + i + "].RoleID' name='UserRoles[" + i + "].RoleID' type='hidden' value='" + role.RoleID + "' ><input type='hidden'  value='false' name='UserRoles[" + i + "].IsAssigned'><input type='checkbox' id='UserRoles[" + i + "].IsAssigned' value='true' name='UserRoles[" + i + "].IsAssigned'><label for='UserRoles[" + i + "].IsAssigned'>" + role.RoleName + "</label></div>"
        //        //    $('#rolename', 'div').append(rolename);


        //        //});
        //    },
        //    error: function (err) {
        //        alert(err);
        //        //console.log(err);
        //    },
        //});

        $.post("/Students/StudentParentsSave", { "studentID": $("#StudentID").val(), "parents": obj  }).done(function (res) {
            
            if (res.status == true) {
                $('#ParentID').val(res.id);
                }
                alert(res.resMessage);

        }).fail(function (ex) {
            
            alert(ex.responseText);
            }).always(function () { });

        //$.ajax({
        //    url: "/Students/StudentParentsSave",
        //    type: "POST",
        //    data: jsonData,
        //    contentType: "application/json; charset=UTF-8",
        //    dataType: "JSON",
        //    success: function (res) {
        //        
        //        if (res.status == true) {
        //            $('#ParentID').val(res.resObj.ParentID);

        //        }
        //        alert(res.resMessage);
        //    },
        //    error: function (jqXHR, textStatus, errorThrown) {
        //        
        //        alert(errorThrown);
        //    }
        //});

    });
    

    $(".showError").change(function () {
        $(this).next().css("display", "none");
    });

    $("#GuardianTypeID").trigger("change");

    $("#GuardianTypeID").change(function () {

        var option = $("#GuardianTypeID").val();
        prtEnableGuardian(option);
    });

    $("#prtStatusTypeID").change(function () {
        
        var status = $("#prtStatusTypeID").val();
        if (status == 3) {
            $("#dvLeftDate").css("display", "block");
        }
        else {
            $("#dvLeftDate").css("display", "none");
        }
    });

});

function prtCheckExist(chk) {
    if ($("#chkPExist").is(":checked")) {
        $("#ParentID").prop("readonly", false);
        $("#btnParentSearch").prop("disabled", false);
    }
    else {
        $("#ParentID").prop("readonly", true);
        $("#btnParentSearch").prop("disabled", true);
    }
}

function prtCancel() {
    
    prtClearControls();
    var parentID = $("#hdParentID").val();
    if (parentID == '' || parentID == undefined || parentID == 0) {
        prtPageLoad();
    }
    else {
        prtGetParentData(parentID);
    }
}

function prtPageLoad() {
    
    $("#ParentID").prop("readonly", true);
    $("#btnParentSearch").prop("disabled", true);
    $("#chkPExist").prop("checked", false);

    var status = $("#prtStatusTypeID").val();
    if (status == 3) {
        $("#dvLeftDate").css("display", "block");
    }
    else {
        $("#dvLeftDate").css("display", "none");
    }
}

function prtEnableGuardian(opt) {
    if (opt == 3) {
        $("#GuardianName").attr('readonly', false);
        $("#GuardianRelation").attr('readonly', false);
    }
    else {
        $("#GuardianName").attr('readonly', true).val('');
        $("#GuardianRelation").attr('readonly', true).val('');

    }
}

function prtIsValidParent() {
    var valid = true;
    $(".error").css("display", "none");

    if ($("#FatherName").val() == "") {
        $("#spnFatherNameError").css("display", "block");
        $("#spnFatherNameError").css("color", "red");
        valid = false;
    }

    if ($("#GuardianTypeID").val() == 3) {
        if ($("#GuardianName").val() == "") {
            $("#spnGuardianNameError").css("display", "block");
            $("#spnGuardianNameError").css("color", "red");
            valid = false;
        }
    }

    if ($("#Phone1Msg").val() == true && $("#Phone1").val() == '') {
        $("#spnPhone1Error").css("display", "block");
        $("#spnPhone1Error").css("color", "red");
        valid = false;
    }

    if ($("#Phone2Msg").val() == true && $("#Phone2").val() == '') {
        $("#spnPhone2Error").css("display", "block");
        $("#spnPhone1Error").css("color", "red");
        valid = false;
    }

    if ($("#Address").val() == "") {
        $("#spnAddressError").css("display", "block");
        $("#spnAddressError").css("color", "red");
        valid = false;
    }

    return valid;
}

function prtprtGetParentDataObject() {
    
    var obj = {
        ParentID: $("#ParentID").val(),
        FatherName: $("#FatherName").val(),
        MotherName: $("#MotherName").val(),
        GuardianName: $("#GuardianName").val(),
        GuardianRelation: $("#GuardianRelation").val(),
        GuardianTypeID: $("#GuardianTypeID").val(),
        NIC: $("#NIC").val(),
        Profession: $("#Profession").val(),
        Email: $("#Email").val(),
        Phone1: $("#Phone1").val(),
        Phone1Msg: false,// $("#Phone1Msg").val(),
        Phone2: $("#Phone2").val(),
        Phone2Msg: false,// $("#Phone2Msg").val(),
        Address: $("#Address").val(),
        SchoolID: $("#SchoolID").val(),

    };


    return obj;
}

function prtGetParentData(id) {
    
    var ParentID;
    if (id == undefined || id == null || id == 0) {
        ParentID = $("#ParentID").val();
    }
    else {
        ParentID = id;
    }

    
    prtClearControls();

    if (ParentID != undefined && ParentID != null && ParentID > 0) {
        var jsonData = JSON.stringify({ "ParentID": ParentID });

        $.ajax({
            url: "/Students/GetParent",
            data: jsonData,
            type: "POST",
            contentType: "application/json",
            dataType: "JSON",
            success: function (data) {
                if (data.status == true) {
                    prtPopulateControls(data.resObj);
                    prtPageLoad();
                }
                else {
                    alert("Record no found");
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    }
    else {
        alert("Invalide parent id");
    }
}
function prtClearControls() {
    $("#FatherName").val('');
    $("#MotherName").val('');
    $("#GuardianTypeID").val('');
    $("#GuardianName").val('');
    $("#GuardianRelation").val('');
    //$("#GuardianTypeID").trigger("change");
    $("#NIC").val('');
    $("#Profession").val('');
    $("#Email").val('');
    $("#Phone1").val('');
    $("#Phone2").val('');
    $("#Address").val('');
}

function prtPopulateControls(data) {
    $("#ParentID").val(data.ParentID);
    $("#FatherName").val(data.FatherName);
    $("#MotherName").val(data.MotherName);
    $("#GuardianTypeID").val(data.GuardianTypeID);
    $("#GuardianName").val(data.GuardianName);
    $("#GuardianRelation").val(data.GuardianRelation);
    $("#GuardianTypeID").trigger("change");
    $("#NIC").val(data.NIC);
    $("#Profession").val(data.Profession);
    $("#Email").val(data.Email);
    $("#Phone1").val(data.Phone1);
    $("#Phone2").val(data.Phone2);
    $("#Address").val(data.Address);
}

