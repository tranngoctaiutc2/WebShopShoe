window.onload = function () {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(showWeather, handleError);
    } else {
        console.log("Định vị địa lý không được hỗ trợ bởi trình duyệt này.");
    }
};

function showWeather(position) {
    var latitude = position.coords.latitude;
    var longitude = position.coords.longitude;

    var apiKey = "c7055ce01673bc05bf8af1cb09e60bd2";
    var apiUrl = "https://api.openweathermap.org/data/2.5/weather?lat=" + latitude + "&lon=" + longitude + "&lang=vi&appid=" + apiKey + "&units=metric";

    fetch(apiUrl)
        .then(response => response.json())
        .then(data => {
            console.log(data)
            setInterval(() => {
                document.getElementById("dt").innerText = new Date().toLocaleTimeString();
                document.getElementById("dmy").innerText = new Date().toLocaleDateString("vi-VN");
            }, 1000);
            document.getElementById("location1").innerText = data.name + ", " + data.sys.country;
            document.getElementById("temp_max").innerText = data.main.temp_max + "°C";
            document.getElementById("temp_min").innerText = data.main.temp_min + "°C";
            document.getElementById("humidity").innerText = data.main.humidity + "%";
            document.getElementById("condition").innerText = data.weather[0].description;
            document.getElementById("wind-speed").innerText = data.wind.speed + "m/s";
            var iconCode = data.weather[0].icon;
            document.getElementById("weather-icon").src = "https://openweathermap.org/img/wn/" + iconCode + ".png";
            

        })
        .catch(error => console.error("Lỗi khi tìm nạp dữ liệu thời tiết:", error));

    var oneCallApiUrl = "https://api.openweathermap.org/data/2.5/onecall?lat=" + latitude + "&lon=" + longitude + "&exclude=minutely,hourly,daily,alerts&appid=" + apiKey + "&units=metric";
    fetch(oneCallApiUrl)
        .then(response => response.json())
        .then(data => {

            document.getElementById("uv-index").innerText = data.current.uvi + "nm";
        })
        .catch(error => console.error("Lỗi khi tìm nạp dữ liệu chỉ số:", error));

    var airPollutionApiUrl = "https://api.openweathermap.org/data/2.5/air_pollution?lat=" + latitude + "&lon=" + longitude + "&appid=" + apiKey;
    // Tạo một đối tượng dữ liệu ánh xạ giữa chỉ số ô nhiễm và mức độ phù hợp
    var pollutionLevels = {
        1: "Tốt",
        2: "Khá",
        3: "Trung Bình",
        4: "Xấu",
        5: "Rất Xấu"
    };

    fetch(airPollutionApiUrl)
        .then(response => response.json())
        .then(data => {
            // Kiểm tra xem dữ liệu có tồn tại hay không
            if (data.list && data.list.length > 0) {
                // Lấy chỉ số ô nhiễm không khí hiện tại (chỉ lấy cho một trong các thành phần trong list)
                var currentAirPollutionIndex = data.list[0].main.aqi;
                // Hiển thị chỉ số ô nhiễm không khí
                document.getElementById("air-pollution-index").innerText = "Mức ô nhiễm: " + currentAirPollutionIndex + " (" + pollutionLevels[currentAirPollutionIndex] + ")";
            } else {
                document.getElementById("air-pollution-index").innerText = "Không có dữ liệu";
            }
        })
        .catch(error => console.error("Lỗi khi tìm nạp dữ liệu ô nhiễm không khí:", error));
}
function handleError(error) {
    console.error("Lỗi nhận vị trí địa lý:", error.message);
}

function updateBackground() {
    var currentTime = new Date();
    var hours = currentTime.getHours();

    if (hours >= 6 && hours < 18) {
        document.querySelector('.box_s').style.background = 'linear-gradient(0deg, #a5a44e 0%, #f0952d 80%)';
    } else {
        document.querySelector('.box_s').style.background = 'linear-gradient(0deg, #2d454d 0%, #40397a 80%)';
        9
    }
}
