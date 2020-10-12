function clearFunction() {
	document.getElementById("myList").selectedIndex = "All"
	document.getElementById("myInput").value = ''
	document.getElementById("perfIncrease").checked = false
	document.getElementById("perfDecrease").checked = false
	change()
	filterFunction()
}

function perfIncFilter() {
	var input, filter, table, tr, td, i;
	if (document.getElementById("perfDecrease").checked == true) {
		document.getElementById("perfDecrease").checked = false
	}
	if (document.getElementById("perfIncrease").checked == true) {
		document.getElementById("myList").selectedIndex = "All"
		change()
		table = document.getElementById("myTable")
		tr = table.getElementsByTagName("tr")
		filter = "#ffbd9a"
		for (i = 0; i < tr.length; i++) {
			td = tr[i].getElementsByTagName("th")[2]
			var xx = td.getAttribute('style').split(';').filter(item => item.startsWith('background-color'))[0]
			try {
				var yy = xx.split(":")[1];
				if (yy == ' #e3ffd5') {
					tr[0].style.display = ""
					tr[i].style.display = ""
				}
				else {
					tr[0].style.display = ""
					tr[i].style.display = "none"
				}
			}
			catch
			{
				tr[0].style.display = "";
				tr[i].style.display = "none";
			}
		}
	}
	if (document.getElementById("perfIncrease").checked == false) {
		clearFunction();
	}
}

function perfDecFilter() {
	var input, filter, table, tr, td, i;
	if (document.getElementById("perfIncrease").checked == true) {
		document.getElementById("perfIncrease").checked = false
	}

	if (document.getElementById("perfDecrease").checked == true) {
		document.getElementById("myList").selectedIndex = "All";
		change();
		table = document.getElementById("myTable");
		tr = table.getElementsByTagName("tr");
		filter = "#ffbd9a";

		for (i = 0; i < tr.length; i++) {
			td = tr[i].getElementsByTagName("th")[2];
			var xx = td.getAttribute('style').split(';').filter(item => item.startsWith('background-color'))[0];
			try {
				var yy = xx.split(":")[1];
				if (yy == ' #ffbd9a') {
					tr[0].style.display = "";
					tr[i].style.display = "";
				}
				else {
					tr[0].style.display = "";
					tr[i].style.display = "none";
				}
			}
			catch
			{
				tr[0].style.display = "";
				tr[i].style.display = "none";
			}
		}
	}

	if (document.getElementById("perfDecrease").checked == false) {
		clearFunction();
	}
}

function filterFunction() {
	var input, filter, table, tr, td, i, txtValue;
	input = document.getElementById("myInput");
	var listbox = document.getElementById("myList");
	var selIndex = listbox.selectedIndex;
	var selValue = listbox.options[selIndex].value;

	if (selValue != "All") {
		filter = selValue.toUpperCase()
	}

	if (selValue == "All") {
		filter = input.value.toUpperCase()
	}

	table = document.getElementById("myTable");
	tr = table.getElementsByTagName("tr");

	for (i = 0; i < tr.length; i++) {
		td = tr[i].getElementsByTagName("th")[0];
		if (td) {
			txtValue = td.textContent || td.innerText;
			if (txtValue.toUpperCase().indexOf(filter) > -1) {
				tr[0].style.display = "";
				tr[i].style.display = "";
			}
			else {
				tr[0].style.display = "";
				tr[i].style.display = "none";
			}
		}
	}

	document.getElementById("perfIncrease").checked = false;
	document.getElementById("perfDecrease").checked = false
}