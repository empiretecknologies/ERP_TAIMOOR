$(document).ready(function () {
	var $uploadCrop,
		tempFilename,
		rawImg,
		imageId,
		imageName;
	function readFile(input) {
		if (input.files && input.files[0]) {
			var reader = new FileReader();
			reader.onload = function (e) {
				$('.upload-demo').addClass('ready');
				$('#cropImagePop').modal('show');
				rawImg = e.target.result;
			}
			reader.readAsDataURL(input.files[0]);
		}
		else {
			console.log("Sorry - you're browser doesn't support the FileReader API");
		}
	}

	$uploadCrop = $('#upload-demo').croppie({
		viewport: {
			width: 200,
			height: 200,
			type: 'circle'
		},
		enforceBoundary: false,
		enableExif: true
	});
	$('#cropImagePop').on('shown.bs.modal', function () {
		$('.cr-slider-wrap').prepend('<p>Image Zoom</p>');
		$uploadCrop.croppie('bind', {
			url: rawImg
		}).then(function () {
			console.log('jQuery bind complete');
			$uploadCrop.croppie('setZoom', 1)
		});

		
	});

	$('#cropImagePop').on('hidden.bs.modal', function () {
		$('.item-img').val('');
		$('.cr-slider-wrap p').remove();
	});

	$('#Image').on('change', function () {
		readFile(this);
	});
	$('#WaterImage').on('change', function () {
		readFile(this);
		imageName = "Water";
	});
	$('#GroupImage').on('change', function () {
		readFile(this);
		imageName = "ItemGroup";
	});
	$('#ItemImage').on('change', function () {
		readFile(this);
		imageName = "ItemMaster";
	});
	$('#WaiterImage').on('change', function () {
		readFile(this);
		imageName = "Waiter";
	});
	$('#TableImage').on('change', function () {
		readFile(this);
		imageName = "Table";
	});
	
	$('#PosLogo').on('change', function () {
		readFile(this);
		imageName = "PosLogo";
	});

	$('.replacePhoto').on('click', function () {
		$('#cropImagePop').modal('hide');
		$('#Image').val('');
	})

	$('#cropImageBtn').on('click', function (ev) {
		$uploadCrop.croppie('result', {
			type: 'base64',
			// format: 'jpeg',
			backgroundColor: "#000000",
			format: 'png',
			size: { width: 400, height: 400 }
		}).then(function (resp) {
			$('#item-img-output').attr('src', resp);
			$('#cropImagePop').modal('hide');
			$('.item-img').val('');
			if (window.location.href.includes('ItemMaster')) {
				empr_ItemMaster.UploadImage();
			}
			if (window.location.href.includes('ItemGroup')) {
				empr_ItemGroups.UploadImage();
			}
			if (window.location.href.includes('Branch')) {
				empr_Branch.SaveImage();
			}
			if (window.location.href.includes('User')) {
				empr_User.SaveImage();
			}
			if (window.location.href.includes('Company')) {
				empr_Company.SaveImage(imageName);
			}
			if (window.location.href.includes('Employee')) {
				empr_Employee.SaveImage();
			}
			if (window.location.href.includes('POSMapping')) {
				empr_POSMapping.SaveImage(imageName);
			}
			else {
				empr_SetupSubType.SaveImage();
			}
		});
	});      
});