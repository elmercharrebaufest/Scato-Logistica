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

					if (isDateTime) {
						$('#' + element.id).attr("placeholder", "dd/mm/aaaa hh:mm:ss");
						$('#' + element.id).datetimepicker({
							format: 'dd/mm/yyyy hh:ii:ss',
							autoclose: true,
						});
					} else if (isDate) {
						$('#' + element.id).attr("placeholder", "dd/mm/aaaa");
						$('#' + element.id).datetimepicker({
							format: 'dd/mm/yyyy hh:ii:ss',
							autoclose: true,
							minView: 2
						});
						element.value = obtainDateOnly(element.value);

						$(element).on('hide', function (e) {
							var ele = this;
							ele.value = obtainDateOnly(ele.value);
						})
					}


					$('#' + element.id).attr("readonly", "readonly");
					$('#' + element.id).attr("autocomplete", "off");
				});
			}

			function obtainDateOnly(fulldate) {
				if (fulldate != "" && fulldate != null) {
					var dateArray = fulldate.split(" ");
					if (dateArray.length > 1) {
						fulldate = dateArray[0] + " 00:00:00";
					} else {
						fulldate = fulldate + " 00:00:00"
					}
				}
				return fulldate;
			}
		}
	});

})();