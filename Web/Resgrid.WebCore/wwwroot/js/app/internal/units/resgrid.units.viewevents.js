
var resgrid;
(function (resgrid) {
    var units;
    (function (units) {
        var viewevents;
        (function (viewevents) {
            $(document).ready(function () {
                var mapOptions = {
                    center: new google.maps.LatLng(43, 16),
                    zoom: 11,
                    mapTypeId: google.maps.MapTypeId.ROADMAP
                };
                map = new google.maps.Map(document.getElementById('eventsMap'), mapOptions);
                markers = [];
                $("#eventsGrid").kendoGrid({
                    dataSource: {
                        type: "json",
                        transport: {
                            read: resgrid.absoluteBaseUrl + '/User/Units/GetUnitEvents?UnitId=' + purl().param('unitId')
                        },
                        schema: {
                            model: {
                                fields: {
                                    EventId: { type: "number" },
                                    Name: { type: "string" },
                                    Action: { type: "string" },
                                    Timestamp: {
                                        type: "string"
                                    }
                                }
                            }
                        },
                        pageSize: 50,
                        serverPaging: false,
                        serverFiltering: false,
                        serverSorting: false
                    },
                    height: 650,
                    filterable: true,
                    sortable: true,
                    pageable: true,
                    dataBound: onDataBound,
                    columns: [
                        {
                            field: "EventId",
                            title: "",
                            width: 15,
                            filterable: false,
                            sortable: false,
                            headerTemplate: '<label><input type="checkbox" id="checkAllEvents"/></label>',
                            template: "<input type=\"checkbox\" id=\"selectEvent_#=EventId#\" name=\"selectEvent_#=EventId#\" />"
                        },
                        {
                            field: "State",
                            title: "State",
                            width: 80
                        },
                        {
                            field: "DestinationName",
                            title: "Destination",
                            width: 100
                        },
                        {
                            field: "Timestamp",
                            title: "Timestamp",
                            width: 100,
                            format: "{0:MM/dd/yyyy HH:mm:ss}"
                        },
                        {
                            field: "Note",
                            title: "Note",
                            width: 100
                        }
                    ]
                });
                $('#checkAllEvents').on('click', function () {
                    $('#eventsGrid').find(':checkbox').prop('checked', this.checked);
                });
                function onDataBound() {
                    var ds = $("#eventsGrid").data("kendoGrid").dataSource;
                    //var filters = ds.filter();
                    //var allData = ds.data();
                    var query = new kendo.data.Query(ds.data());
                    var filteredData = query.filter(ds.filter()).data;
                    $.each(markers, function (index, item) {
                        item.setMap(null);
                    });
                    markers = [];
                    $.each(filteredData, function (index, item) {
                        if (item.Latitude && item.Latitude.length > 0 && item.Longitude && item.Longitude.length > 0) {
                            var latLng = new google.maps.LatLng(item.Latitude, item.Longitude);
                            var marker = new google.maps.Marker({
                                position: latLng,
                                map: map,
                                title: item.State + ' ' + item.Timestamp,
                                //title: "",
                                icon: "/Images/Mapping/Event.png"
                            });
                            markers.push(marker);
                        }
                    });
                    var latlngbounds = new google.maps.LatLngBounds();
                    $.each(markers, function (index, item) {
                        var latLng = new google.maps.LatLng(item.getPosition().lat(), item.getPosition().lng());
                        latlngbounds.extend(latLng);
                    });
                    map.setCenter(latlngbounds.getCenter());
                    map.fitBounds(latlngbounds);
                    //var zoom = map.getZoom();
                    //map.setZoom(zoom > zoomLevel ? that.zoomLevel : zoom);
                }
            });
        })(viewevents = units.viewevents || (units.viewevents = {}));
    })(units = resgrid.units || (resgrid.units = {}));
})(resgrid || (resgrid = {}));
