$(document).ready(function () {
	function ChildMenu(Id) {
		debugger;
		if (Id !== null) {
			$.ajax({
				url: "/Base/GetChildRecord",
				type: "GET",
				data: { Id: Id },
				success: function (data) {
					console.log(data);
					$.each(data, function (index, value) {
						// Handle data here
					});
				},
				error: function (xhr, status, error) {
					// Handle error if needed
					console.error(error);
				}
			});
		}
	}
});