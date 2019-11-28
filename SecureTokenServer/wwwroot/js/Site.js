$(function () {
    var modalContainer = $('#modal-container');
    $('button[data-toggle="ajax-modal"]').click(function (event) {
        var url = $(this).data('url');
        $.get(url).done(function (data) {
            $('#modal-container').html(data);
            $('#modal-container > .modal').modal('show');
        });

        modalContainer.on('click', '[data-save="modal"]', function (event) {
            event.preventDefault();
            var form = $(this).parents('.modal').find('form');
            var actionUrl = form.attr('action');
            var dataToSend = form.serialize();

            $.post(actionUrl, dataToSend).done(function (data) {
                console.log(data.success);
                if (data.success === false) {
                    alert(data.errorMessage);
                }
                else {
                    $('#createScopeModal').modal('hide');
                    location.reload();
                }
            });
        });
    });

    $('button[data-toggle="manage-modal"]').click(function (event) {
        var url = $(this).data('url');
        $.get(url).done(function (data) {
            $('#modal-container').html(data);
            $('#modal-container > .modal').modal('show');
        });

        modalContainer.on('click', '[data-save="modal"]', function (event) {
            event.preventDefault();
            var form = $(this).parents('.modal').find('form');
            var actionUrl = form.attr('action');
            var dataToSend = form.serialize();

            $.post(actionUrl, dataToSend).done(function (data) {
                $('#createScopeModal').modal('hide');
                location.reload();
            });
        });

        modalContainer.on('click', '[data-delete="claim"]', function (event) {
            event.preventDefault();
            var url = $(this).data('url');
            $.post(url).done(function (data) {
                $('#createScopeModal').modal('hide');
                location.reload();
            });
        });
    });

    $('button[data-toggle="delete-modal"]').click(function (event) {
        var url = $(this).data('url');
        $.get(url).done(function (data) {
            $('#modal-container').html(data);
            $('#modal-container > .modal').modal('show');
        });

        modalContainer.on('click', '[data-save="modal"]', function (event) {
            event.preventDefault();
            var form = $(this).parents('.modal').find('form');
            var actionUrl = form.attr('action');
            var dataToSend = form.serialize();

            $.post(actionUrl, dataToSend).done(function (data) {
                $('#createScopeModal').modal('hide');
                location.reload();
            });
        });
    });

    $('button[data-toggle="delete-resource"]').click(function (event) {
        var url = $(this).data('url');
        $.get(url).done(function (data) {
            $('#modal-container').html(data);
            $('#modal-container > .modal').modal('show');
        });

        modalContainer.on('click', '[data-save="modal"]', function (event) {
            event.preventDefault();
            var form = $(this).parents('.modal').find('form');
            var actionUrl = form.attr('action');
            var dataToSend = form.serialize();

            $.post(actionUrl, dataToSend).done(function (data) {
                $('#createScopeModal').modal('hide');
                location.reload();
            });
        });
    });

    $('button[data-toggle="add-client-resource"]').click(function (event) {
        var url = $(this).data('url');
        $.get(url).done(function (data) {
            console.log(data);
            $('#modal-container').html(data);
            $('#modal-container > .modal').modal('show');
        });

        modalContainer.on('click', '[data-save="modal"]', function (event) {
            event.preventDefault();
            var form = $(this).parents('.modal').find('form');
            var actionUrl = form.attr('action');
            var dataToSend = form.serialize();

            $.post(actionUrl, dataToSend).done(function (data) {
                if (data.success === false) {
                    alert(data.errorMessage);
                }
                else {
                    alert("in");
                    $('#modal-container').modal('hide');
                    location.reload();
                }
            });
        });
    });

    $('button[data-toggle="delete-client-resource"]').click(function (event) {
        var url = $(this).data('url');
        $.get(url).done(function (data) {
            $('#modal-container').html(data);
            $('#modal-container > .modal').modal('show');
        });

        modalContainer.on('click', '[data-save="modal"]', function (event) {
            event.preventDefault();
            var form = $(this).parents('.modal').find('form');
            var actionUrl = form.attr('action');
            var dataToSend = form.serialize();

            $.post(actionUrl, dataToSend).done(function (data) {
                $('#modal-container').modal('hide');
                location.reload();
            });
        });
    });

  
});


function manageResource(checkbox,checked) {
    console.log(checkbox);
    var url = checkbox.getAttribute('data-url');
    url = url + '&assignClaim=' +checked;
    console.log(url);

    $.post(url).done(function (data) {       
        location.reload();
    });
}