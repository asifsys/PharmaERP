$(document).ready(function () {

    let serviceTable;

    // Load service information from MVC endpoint.
    function loadDashboard() {

        $.ajax({
            url: '/Dashboard/GetServices',
            type: 'GET',

            success: function (services) {

                updateSummary(services);

                populateDepartmentFilter(services);

                if (serviceTable) {
                    serviceTable.clear();
                    serviceTable.rows.add(services);
                    serviceTable.draw();
                }
                else {
                    initializeGrid(services);
                }

                $('#lastUpdated').text(
                    'Last updated: ' +
                    new Date().toLocaleTimeString()
                );
            },

            error: function () {
                $('#lastUpdated').text(
                    'Unable to retrieve service information'
                );
            }
        });
    }

    // Update four executive KPI tiles.
    function updateSummary(services) {

        $('#totalServices').text(services.length);

        $('#healthyServices').text(
            services.filter(x => x.status === 'Healthy').length
        );

        $('#degradedServices').text(
            services.filter(x => x.status === 'Degraded').length
        );

        $('#offlineServices').text(
            services.filter(x => x.status === 'Offline').length
        );
    }

    // Dynamically create department filter options.
    function populateDepartmentFilter(services) {

        const select = $('#departmentFilter');

        const currentValue = select.val();

        const departments = [
            ...new Set(services.map(x => x.department))
        ];

        select.empty();

        select.append(
            $('<option>', {
                value: '',
                text: 'All Departments'
            })
        );

        departments.forEach(function (department) {

            select.append(
                $('<option>', {
                    value: department,
                    text: department
                })
            );
        });

        select.val(currentValue);
    }

    // Initialize DataTables.
    function initializeGrid(services) {

        serviceTable = $('#serviceTable').DataTable({

            data: services,

            pageLength: 10,

            lengthMenu: [10, 25, 50, 100],

            searching: true,

            ordering: true,

            responsive: true,

            columns: [

                {
                    data: 'serviceName'
                },

                {
                    data: 'department'
                },

                {
                    data: 'status',

                    render: function (data) {

                        const statusClass = {
                            Healthy: 'bg-success',
                            Degraded: 'bg-warning text-dark',
                            Offline: 'bg-danger'
                        };

                        const cssClass =
                            statusClass[data] || 'bg-secondary';

                        return `<span class="badge ${cssClass}">
                                    ${escapeHtml(data)}
                                </span>`;
                    }
                },

                {
                    data: 'responseTimeMs',

                    render: function (data) {
                        return `${Number(data).toFixed(2)} ms`;
                    }
                },

                {
                    data: 'cpuUsage',

                    render: function (data) {
                        return `${Number(data).toFixed(1)}%`;
                    }
                },

                {
                    data: 'memoryUsageMB',

                    render: function (data) {
                        return `${Number(data).toFixed(1)} MB`;
                    }
                },

                {
                    data: 'lastChecked',

                    render: function (data) {
                        return data
                            ? new Date(data).toLocaleString()
                            : '-';
                    }
                }
            ]
        });
    }

    // Escape values before displaying dynamic text.
    function escapeHtml(value) {
        return $('<div>').text(value ?? '').html();
    }

    // Search service or department.
    $('#serviceSearch').on('input', function () {

        if (serviceTable) {
            serviceTable.search(this.value).draw();
        }
    });

    // Department filter.
    $('#departmentFilter').on('change', function () {

        if (serviceTable) {
            serviceTable.column(1).search(this.value).draw();
        }
    });

    // Status filter.
    $('#statusFilter').on('change', function () {

        if (serviceTable) {
            serviceTable.column(2).search(this.value).draw();
        }
    });

    // Clear filters.
    $('#clearFilters').on('click', function () {

        $('#serviceSearch').val('');
        $('#departmentFilter').val('');
        $('#statusFilter').val('');

        if (serviceTable) {
            serviceTable.search('');
            serviceTable.columns().search('');
            serviceTable.draw();
        }
    });

    // Manual refresh.
    $('#refreshDashboard').on('click', function () {
        loadDashboard();
    });

    // Initial page load.
    loadDashboard();

    // Refresh service health every 30 seconds.
    setInterval(loadDashboard, 30000);

});