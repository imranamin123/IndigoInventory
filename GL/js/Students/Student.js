

$().ready(function () {
    $("#SectionID").change(function (e) {

        e.preventDefault;
        getStudentBySection();
    });

    $(".saveStudent").click(function (e) {

        e.preventDefault;
        if (!IsValidStudent() ) {
            return;
        }
        var obj = getStudentObject();
        var jsonData = JSON.stringify({ "student": obj });

        $.ajax({
            url: "/Students/StudentSave",
            type: "POST",
            data: jsonData,
            contentType: "application/json; charset=UTF-8",
            dataType: "JSON",
            success: function (id) {
                
                if (parseInt(id) > 0) {
                    alert("Record saved successfully!");
                    var url = "/Students/Student/" + parseInt(id);
                    window.location = url;

                }
                else {
                    alert("Error occured in saving");
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });

    });

    $(".showError").change(function () {
        $(this).next().css("display", "none");
    });

    //$("#ClassID").change(function (e) {

    //    e.preventDefault;
    //    getSectionByClass();
    //});

});



function IsValidStudent() {
    var valid = true;
    $(".error").css("display", "none");

    if ($("#StudentName").val() == "") {
        $("#spnStudentNameError").css("display", "block");
        valid = false;
    }
    if ($("#RollNo").val() == "") {
        $("#spnRollNoError").css("display", "block");
        valid = false;
    }

    if ($("#AdmissionClassID option:selected").index() == "0") {
        $("#spnAdmissionClassIDError").css("display", "block");
        valid = false;
    }

    if ($("#ClassID option:selected").index() == "0") {
        $("#spnClassIDError").css("display", "block");
        valid = false;
    }
    if ($("#SectionID option:selected").index() == "0") {
        $("#spnSectionIDError").css("display", "block");
        valid = false;
    }
    //if ($("#SessionID option:selected").index() == "0") {
    //    $("#spnSessionIDError").css("display", "block");
    //    valid = false;
    //}
    if ($("#DateOfBirth").val() == "") {
        $("#spnDateOfBirthError").css("display", "block");
        valid = false;
    }
    if ($("#AdmissionDate").val() == "") {
        $("#spnAdmissionDateError").css("display", "block");
        valid = false;
    }

    if ($("#GenderID option:selected").index() == "0") {
        $("#spnGenderIDError").css("display", "block");
        valid = false;
    }

    if ($("#StatusTypeID").val()=="") {
        $("#spnStatusTypeIDError").css("display", "block");
        valid = false;
    }
    

    return valid;
}

function getStudentObject() {
    var obj = {
        SchoolID:  $("#hdSchoolID").val(),
        StudentID: $("#StudentID").val(),
        RollNo: $("#RollNo").val(),        
        StudentName: $("#StudentName").val(),
        SessionID: $("#SessionID").val(),
        AdmissionClassID: $("#AdmissionClassID").val(),
        ClassID: $("#ClassID").val(),
        SectionID: $("#SectionID").val(),
        DateOfBirth: $("#DateOfBirth").val(),
        AdmissionDate: $("#AdmissionDate").val(),
        LeftDate: $("#LeftDate").val(),
        GenderID: $("#GenderID").val(),
        Remarks: $("#Remarks").val(),
        //Parents: null,
        ParentID: null,// $("#ParentID").val(),
        //hidden fields
        CreatedBy: $("#hdCreatedBy").val(),
        CreatedAt: $("#hdCreatedAt").val(),
        ModifiedBy: $("#hdModifiedBy").val(),
        ModifiedAt: $("#hdModifiedAt").val(),
        ImagePath: $("#hdImagePath").val(),
        //
        StatusTypeID: $("#StatusTypeID").val(),
       // SchoolID: $("#SchoolID").val()

    }
    
    return obj;
}


