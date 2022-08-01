function ajaxCall2(method, api, data, successCB, errorCB) {
    $.ajax({
        type: method,
        url: api,
        data: data,
        cache: false,
        headers: {
            'x-rapidapi-key': '7505d2f255msh77e8ac6cf041f42p1f0554jsn2dacb72e6683',   // replace it with your own key
        },
        contentType: "application/json",
        dataType: "json",
        success: successCB,
        error: errorCB
    });
}