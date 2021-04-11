// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
let p = document.querySelector('#createajax');
//document.querySelector('#fetch').addEventListener('click', callBack);
function callBack() {
    fetch('https://localhost:44355/GymClasses/fetch', {
        method: 'Get',
        headers: {
            'X-ReQuested-With': 'XMLHttpRequest'
        }
    })
        .then(res => res.text())
        .then(data => {
            p.innerHTML = data;
        })
        .catch(err => console.log(err));
}
$('#fetch').click(function () {
    $.ajax({
        url: 'https://localhost:44355/GymClasses/fetch',
        type: 'GET',
        success: success,
        failure: fail
    });
});
function success(response) {
    if (200 == response.status) {
        console.log('OK')
    }
    p.innerHTML = response;
}

function removeForm() {
    $('#createajax').remove();
}
function fail() {
    console.log('fail');
}
