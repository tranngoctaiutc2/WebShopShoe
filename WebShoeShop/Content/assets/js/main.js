/*price range*/

 $('#sl2').slider();

	var RGBChange = function() {
	  $('#RGB').css('background', 'rgb('+r.getValue()+','+g.getValue()+','+b.getValue()+')')
	};	
		

window.onscroll = function () {
	toggleBackToTopButton();
};

function toggleBackToTopButton() {
	var button = document.querySelector('.back-to-top');
	var button1 = document.querySelector('.back-to-top1');
	if (document.body.scrollTop > 20 || document.documentElement.scrollTop > 20) {
		button.classList.add('show');
		button1.classList.add('show');
	} else {
		button.classList.remove('show');
		button1.classList.add('show');
	}
}

function scrollToTop() {
	scrollToTopAnimation(0, 500); // Scroll to top with animation, duration: 500ms
}

function scrollToTopAnimation(to, duration) {
	const start = window.scrollY || document.documentElement.scrollTop;
	const change = to - start;
	const increment = 20;
	let currentTime = 0;

	function easeInOutQuad(t, b, c, d) {
		t /= d / 2;
		if (t < 1) return c / 2 * t * t + b;
		t--;
		return -c / 2 * (t * (t - 2) - 1) + b;
	}

	function animateScroll() {
		currentTime += increment;
		const val = easeInOutQuad(currentTime, start, change, duration);
		window.scrollTo(0, val);
		if (currentTime < duration) {
			requestAnimationFrame(animateScroll);
		}
	}

	requestAnimationFrame(animateScroll);
}
var isPartialViewShown = false;
var partialView = document.getElementById("partial-view-container");
var overlay = document.getElementById("overlay");

overlay.style.display = "none";

function showPartialView() {
	if (!isPartialViewShown) {
		partialView.style.display = "block";
		overlay.style.display = "block";
		isPartialViewShown = true;
		positionPartialView();
	} else {
		partialView.style.display = "none";
		overlay.style.display = "none";
		isPartialViewShown = false;
	}
}

function positionPartialView() {
	var windowHeight = window.innerHeight;
	var windowWidth = window.innerWidth;
	var partialViewHeight = partialView.offsetHeight;
	var partialViewWidth = partialView.offsetWidth;
	var topPosition = windowHeight * 0.3; // Điều chỉnh vị trí top dựa trên tỷ lệ của chiều cao màn hình
	var leftPosition = (windowWidth - partialViewWidth) / 2; // Mặc định giữ nguyên vị trí left
	partialView.style.top = 280 + "px";
	partialView.style.left = 700 + "px";
	overlay.style.height = windowHeight + "px";
}

window.addEventListener("resize", positionPartialView);