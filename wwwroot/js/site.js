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
function calculateMonthsBetween(fromDate, toDate) {
    if (fromDate instanceof Date && toDate instanceof Date && !isNaN(fromDate) && !isNaN(toDate)) {
        var months = (toDate.getFullYear() - fromDate.getFullYear()) * 12;
        months -= fromDate.getMonth();
        months += toDate.getMonth();
        // Check if toDate is after the end of the fromDate month
        if (toDate.getDate() >= fromDate.getDate()) {
            months++;
        }
        // Ensure the months value is non-negative
        months = months > 0 ? months : 0;
        return months;
    } else {
        return 0;
    }
}