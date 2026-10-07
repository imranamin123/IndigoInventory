$().ready(function () {

    $("input").prop("autocomplete", "off");
    //$("#SessionID")
    $(document).on('change', '#SupplierID', function (e) {
        $('#SupplierParam').val($("#SupplierID option:selected").text());
    });

    //$(".decimal").on("keypress keyup blur", function (event) {
    $(document).on('keypress keyup blur', '.decimal', function (event) {
        //this.value = this.value.replace(/[^0-9\.]/g,'');
        $(this).val($(this).val().replace(/[^0-9\.]/g, ''));
        if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
            event.preventDefault();
        }
    });

    //$(".integer").on("keypress keyup blur", function (event) {
    $(document).on('keypress keyup blur', '.integer', function (event) {
        $(this).val($(this).val().replace(/[^\d].+/, ""));
        if ((event.which < 48 || event.which > 57)) {
            event.preventDefault();
        }
    });

    $().ready(function () {

        $(".dt").datepicker({
            format: 'dd-M-yyyy',
            autoclose: true,
        });
    });

    $(".readonly").attr("disabled", "disabled");

    //$("#ClassID").change(function (e) {
    $(document).on('change', '#ClassID', function (e) {
        e.preventDefault;

        var index = $("select#ClassID")[0].selectedIndex;
        if (index == 0) {
            $("#SectionID").html("");
            $("#SectionID").html("<option value>- Please Select Section -</option>");
        }
        else {
            var classId = $("#ClassID").val();
            var jsonData = JSON.stringify({ "ClassID": classId });

            $.post("/Setup/GetSectionList", { "ClassID": classId }).done(function (res) {
                $("#SectionID").html("");
                $("#SectionID").html("<option value=''>- Select Section -</option>");
                if (res.status == true) {
                    $(res.resObj).each(function (index, element) {
                        var SectionID = $(this)[0].SectionID;
                        var Name = $(this)[0].Name;
                        var option = "<option value='" + SectionID + "' >" + Name + "</option>";
                        $("#SectionID").append(option);
                    });
                }
                else {

                }
            }).fail(function (ex) {
                alert(ex.responseText);
            }).always(function () { });

        }
    });

    //$("#SectionID").change(function (e) {
    $(document).on('change', '#SectionID', function (e) {

        e.preventDefault;

        //alert('section change');
        var index = $("select#SectionID")[0].selectedIndex;
        if (index == 0) {
            $("#StudentID").html("");
            $("#StudentID").html("<option value=''>- Select Student -</option>");
        }
        else {
            var sectionId = $("#SectionID").val();
            //var jsonData = JSON.stringify({ "SectionID": sectionId });

            $.post("/Setup/GetStudentListBySectionJSON", { "SectionID": sectionId }).done(function (res) {
                $("#StudentID").html("");
                $("#StudentID").html("<option value=''>-Select Student -</option>");

                if (res.status == true) {
                    $(res.resObj).each(function (index, element) {
                        var StudentID = $(this)[0].StudentID;
                        var Name = $(this)[0].StudentName;
                        var option = "<option value='" + StudentID + "' >" + Name + "</option>";
                        $("#StudentID").append(option);
                    });
                }
                else {

                }
            }).fail(function (ex) {
                alert(ex.responseText);
            }).always(function () { });

        }

    });
});

var sidebartoggle = false;
$(document).on('click', '.sidebar-toggle', function (e) {
    if (sidebartoggle == false) {
        $('.main-sidebar').fadeOut();
        $('.content-wrapper').css('margin', 'auto');
        $('.main-footer').css('margin-left', '230px');
        sidebartoggle = true;
    }
    else {
        $('.main-sidebar').fadeIn();
        $('.content-wrapper').css('margin-left', '230px');
        $('.main-footer').css('margin-left', '230px');
        sidebartoggle = false;
    }
})

function PrintReport(url) {
    try {
        $.get(url, function (data) {
            $('#dvHTMLReport').html(data);

            printJS({ printable: 'dvHTMLReport', type: 'html' });

        });
    } catch (ex) {
        console.error(ex);
    }

    

    function refreshSession() {

        setTimeout(function () {
            $.get("/Security/RefreshSession").done(function (res) {

            }).fail(function (ex) {

                alert(ex.responseText);

            }).always(function () {

            });
        }, 2000);
           
        }

    refreshSession();
}

// Loads a list partial into `container` and pages, sorts and searches it on the server.
// The action pages its rows with GL.Common.ServerPaging.Apply and returns the same row
// partial; later pages keep only that partial's <tr>s, so Razor row markup is reused.
function serverPagedList(container, url, filters, options) {
    options = options || {};
    var order = options.order || [[0, "asc"]];
    var pageLength = options.pageLength || 10;

    function load(params) {
        return $.post(url, $.extend({}, filters, params));
    }

    function count(xhr, header) {
        return parseInt(xhr.getResponseHeader(header), 10) || 0;
    }

    load({ PageStart: 0, PageLength: pageLength, PageOrderColumn: order[0][0], PageOrderDir: order[0][1] }).done(function (html, status, xhr) {

        $(container).empty().html(html);

        $(container).find("table").first().DataTable({
            serverSide: true,
            processing: true,
            deferLoading: [count(xhr, "X-Filtered-Count"), count(xhr, "X-Total-Count")],
            searchDelay: 400,
            order: order,
            pageLength: pageLength,
            paging: true,
            lengthChange: true,
            searching: true,
            ordering: true,
            info: true,
            autoWidth: true,
            columnDefs: options.columnDefs || [{ orderable: false, targets: -1 }],
            ajax: function (data, callback) {
                load({
                    PageDraw: data.draw,
                    PageStart: data.start,
                    PageLength: data.length,
                    PageSearch: data.search.value,
                    PageOrderColumn: data.order.length ? data.order[0].column : null,
                    PageOrderDir: data.order.length ? data.order[0].dir : null
                }).done(function (res, s, x) {
                    var rows = $("<div>").append($.parseHTML(res)).find("tbody > tr").map(function () {
                        var cells = $(this).children("td").map(function () { return this.innerHTML; }).get();
                        cells.sourceRow = this;
                        return [cells];
                    }).get();
                    callback({ draw: data.draw, recordsTotal: count(x, "X-Total-Count"), recordsFiltered: count(x, "X-Filtered-Count"), data: rows });
                }).fail(function (ex) {
                    alert(ex.responseText);
                    callback({ draw: data.draw, recordsTotal: 0, recordsFiltered: 0, data: [] });
                });
            },
            // keep the attributes (styles, classes) the partial put on each <tr>/<td>
            createdRow: function (row, cells) {
                var src = cells.sourceRow;
                if (!src) return;
                $.each(src.attributes, function () { row.setAttribute(this.name, this.value); });
                $(src).children("td").each(function (i) {
                    var td = row.cells[i];
                    if (td) $.each(this.attributes, function () { td.setAttribute(this.name, this.value); });
                });
            }
        });

    }).fail(function (ex) {
        alert(ex.responseText);
    });
}
