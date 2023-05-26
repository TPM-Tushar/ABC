$(document).ready(function () {

    $('#demoGrid').DataTable().destroy();
    
    $("#demoGrid").DataTable({

        
        processing: true, // for show progress bar
        serverSide: true, // for process server side
        filter: false, // this is for disable filter (search box)
        orderMulti: false, // for disable multiple column at once
        // "ordering":false,
        //pageLength: 10,
        // "demoGrid_length": 10,
        //"bPaginate": false,
        // paging:false,     
        
        "scrollX": true,

        "autoWidth": false,

        "bJQueryUI": true,

        "ordering": false,    // Ordering (Sorting on Each Column)will Be Disabled

        "info": false,         // Will show "1 to n of n entries" Text at bottom

        "lengthChange": false,// Will Disabled Record number per page
       
        "columnDefs": [{
           
            "className": "text-center",
           
            "targets": [0,1,2,3,4,5,6]
        },
        ],
        "ajax": {
            url: "/SearchCertificate/LoadCertificate",
            data: { "SearchParameters": $("#SearchCertificateForm").serialize() },
            type: "POST",
            
            dataSrc: function (json) {
                
                $('#demoGrid_paginate').hide();
                if (json.errorMessage !== undefined && json.errorMessage != "") {
                    bootbox.alert(json.errorMessage, function () {

                    });
                    return json.data;              
                }
                else {
                    return json.data;
                }
            },
            error: function () {
                // window.location.href = "/Error/Index";
            }
        },

        
        columns: [
              { data: "ApplicationDate", width: "13%"},
              { data: "NameOfEstablishment",width:"10%"},
              { data: "TypeOfEstablishment", width: "10%" },
              { data: "CertificateName", width: "15%" },
              { data: "IssuedOn", width: "14%" },
              { data: "ValidUpto", width: "13%" },
              { data: "AddressString", width: "25%" },
        ]
    });
});