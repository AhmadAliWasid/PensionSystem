// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function awaitHtmlElement(element) {
    $(element).html('<div class="alert alert-warning alert-dismissible"><button type = "button" class= "close" data-dismiss="alert" aria-hidden="true" >×</button><h5><i class="icon fas fa-exclamation-triangle"></i> Wait!</h5> Getting Data from Database. </div >')
}
function startOfMonth(date) {
    return new Date(date.getFullYear(), date.getMonth(), 1);
}
function formatStringDate(myDate) {
    const yourDate = new Date(myDate)
    return yourDate.toISOString().split('T')[0]
}