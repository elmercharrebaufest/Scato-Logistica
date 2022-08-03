(function () {
	// ASP.NET Report Viewer Web Control Enhancements

	$(document).ready(function () {
		// Detect is current browser is IE (MSIE is not used since IE 11)
		var isIE = /MSIE/i.test(navigator.userAgent) || /rv:11.0/i.test(navigator.userAgent);

		function viewerPropertyChanged(sender, e) {
			var viewer = $find("ReportViewer");

			if (e.get_propertyName() === "isLoading" && !viewer.get_isLoading()) {
				var reportViewerHeight = $('#ReportViewer').height();
				var $uiRows = $('#ReportViewer_fixedTable > tbody > tr');
				var controlsHeight = 0;
				$uiRows.each(function (i, el) {
					if (i !== $uiRows.length - 1) {
						controlsHeight += $(el).height();
					}
				});

				var contentAreaHeight = reportViewerHeight - controlsHeight;
				$('#VisibleReportContentReportViewer_ctl09').height(contentAreaHeight);
			}
		}

		function printReport() {
			$find('ReportViewer').exportReport('PDF');
		}

		// 1. Fix Report Scrolling in IE 10 and IE 11
		if (window.hasUserSetHeight && isIE) {
			Sys.Application.add_load(function () {
				var reportViewer = $find("ReportViewer");
				reportViewer.add_propertyChanged(viewerPropertyChanged);
			});
		}

		// 2. Add Print button for non-IE browsers
		if (!isIE && window.showPrintButton) {
			var buttonHtml = $('#non-ie-print-button').html();
			$('#ReportViewer_ctl05 > div').append(buttonHtml);
			$('#PrintButton').click(function (e) {
				e.preventDefault();
				printReport();
			});

			$('#mvcreportviewer-btn-print').hover(function () {
				$(this).css('cursor', 'pointer').css('border', '1px solid rgb(51, 102, 153)').css('background-color', 'rgb(221, 238, 247)');
			}, function () {
				$(this).css('cursor', 'pointer').css('border', '1px solid transparent').css('background-color', 'transparent');
			});
		}

		// FIX DE FECHAS PARA SCATO
		if (!!window.chrome) {

			var observer = new MutationObserver(function (mutations) {
				var changes = false;
				mutations.forEach(function (mutation) {
					if (mutation.removedNodes.length > 0 && changes == false) {
						applyDateFix()
						changes = true;
					}
				})
			});

			observer.observe($("#reportForm")[0], { childList: true, subtree: true });
			applyDateFix();

			function applyDateFix() {
				let datePickers = document.getElementsByClassName('null');
				[...datePickers].forEach((element, index, array) => {

					if (element.nextElementSibling != null && element.nextElementSibling.type == 'image')
						return true;

					if (element.parentElement.parentElement.previousElementSibling.firstChild.textContent.indexOf("Fecha") === -1)
						return true;

					let isDateTime = element.parentElement.parentElement.previousElementSibling.firstChild.textContent.indexOf("Fecha/Hora") !== -1;
					let isDate = element.parentElement.parentElement.previousElementSibling.firstChild.textContent.indexOf("Fecha") !== -1;

					let elemVal = element.value;
					if (elemVal != null && elemVal != "") {
						if (!isValidDate(elemVal)) {
							let newElemVal = "";

							if (isDateTime)
								newElemVal = changeDateTimeFormat(elemVal, true);
							else if (isDate)
								newElemVal = changeDateTimeFormat(elemVal, false);

							element.value = newElemVal;
						}
					}

					if (isDateTime)
						element.type = "datetime-local";
					else if (isDate)
						element.type = "date";

				});
			}

			function isValidDate(dateString) {
				let regEx = /^\d{4}-\d{2}-\d{2}$/;
				if (!dateString.match(regEx)) return false;
				let d = new Date(dateString);
				let dNum = d.getTime();
				if (!dNum && dNum !== 0) return false;
				return d.toISOString().slice(0, 10) === dateString;
			}

			function changeDateTimeFormat(date, showTime) {
				var dateTimeArray = date.split(" ");
				var newDate = dateTimeArray[0];
				var newTime = "";
				if (dateTimeArray.length > 1 && showTime === true) {
					var timeArray = dateTimeArray[1].split(":");
					newTime = " " + timeArray[0] + ":" + timeArray[1];
				}
				let dateArray = newDate.split('/');

				for (var i = 0; i < dateArray.length; i++) {
					if (dateArray[i].length < 2) {
						dateArray[i] = "0" + dateArray[i];
					}
				}
				if (dateArray.length > 1) {
					const [day, month, year] = dateArray;
					const result = [year, month, day].join('-');
					return result + newTime;
				} else {
					return date;
				}

			}
		}
	});

})();