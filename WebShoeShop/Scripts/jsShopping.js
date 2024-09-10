$(document).ready(function () {
    ShowCount();
    $('body').on('click', '.btnAddToCart', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var quatity = 1;
        var tQuantity = $('#quantity_value').text();
        if (tQuantity != '') {
            quatity = parseInt(tQuantity);
        }
        
        //alert(id + " " + quatity);
        $.ajax({
            url: '/shoppingcart/addtocart',
            type: 'POST',
            data: { id: id, quantity: quatity },
            success: function (rs) {
                if (rs.Success) {
                    $('#checkout_items').html(rs.Count);
                    //alert(rs.msg);
                    toastr.success(rs.msg, null, { progressBar: true, positionClass: 'toast-top-right' });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi!',
                        text: 'Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng'
                    });
                }
            }
        });
    });
    $('body').on('click', '.btnUpdate', function (e) {
        e.preventDefault();
        var id = $(this).data("id");
        var quantity = $('#Quantity_' + id).val();
        var size = $('#Size_' + id).val();
        Update(id, quantity, size);

    });
    $('body').on('click', '.btnDeleteAll', function (e) {
        e.preventDefault();
    
        deleteAllProductConfirmation();

    });

    $('body').on('click', '.btnDelete', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        deleteProductConfirmation(id);
    });
    function deleteProductConfirmation(id) {
        Swal.fire({
            title: 'Xóa sản phẩm',
            text: 'Bạn có chắc muốn xóa sản phẩm này khỏi giỏ hàng?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Xóa',
            cancelButtonText: 'Hủy',
        }).then((result) => {
            if (result.isConfirmed) {
                deleteProduct(id);
            }
        });
    }
    function deleteAllProductConfirmation() {
        Swal.fire({
            title: 'Xóa sản phẩm',
            text: 'Bạn có chắc muốn xóa tất cả sản phẩm này khỏi giỏ hàng?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Xóa',
            cancelButtonText: 'Hủy',
        }).then((result) => {
            if (result.isConfirmed) {
                DeleteAll();
            }
        });
    }
    function deleteProduct(id) {
        $.ajax({
            url: '/shoppingcart/Delete',
            type: 'POST',
            data: { id: id },
            success: function (rs) {
                if (rs.Success) {
                    $('#checkout_items').html(rs.Count);
                    $('#trow_' + id).remove();
                    LoadCart();
                }
            }
        });
    }

});
document.addEventListener("DOMContentLoaded", function () {
    const paymentButton = document.querySelector(".btnpayment");

    paymentButton.addEventListener("click", function (e) {
        const quantityInputs = document.querySelectorAll('.quantity-input');
        const sizeInputs = document.querySelectorAll('.size-input');
        let hasInvalidInput = false;

        for (let i = 0; i < quantityInputs.length; i++) {
            const quantityValue = parseInt(quantityInputs[i].value);
            const sizeValue = parseInt(sizeInputs[i].value);

            if (quantityValue === 0 || sizeValue === 0 || isNaN(quantityValue) || isNaN(sizeValue)) {
                hasInvalidInput = true;
                break;
            }
        }

        if (hasInvalidInput) {
            e.preventDefault();

            Swal.fire({
                icon: 'error',
                title: 'Lỗi',
                text: 'Vui lòng chọn số lượng và size hợp lệ!'
            });
        }
    });
});
function updateProduct(element) {
    var inputSizeId = "Size_" + element.dataset.id;
    var inputQuantityId = "Quantity_" + element.dataset.id;
    var inputValue1 = parseFloat(document.getElementById(inputSizeId).value);
    var inputValue2 = parseFloat(document.getElementById(inputQuantityId).value);
    if (isNaN(inputValue1) || isNaN(inputValue2)) {
        Swal.fire({
            icon: 'error',
            title: 'Lỗi!',
            text: 'Vui lòng nhập số lượng và size.'
        });
        document.getElementById(inputSizeId).value = "";
        document.getElementById(inputQuantityId).value = "";
        return;
    }
    if (inputValue1 <= 0 || inputValue2 <= 0) {
        Swal.fire({
            icon: 'error',
            title: 'Giá trị không hợp lệ!',
            text: 'Vui lòng nhập giá trị lớn hơn 0.'
        });
        document.getElementById(inputSizeId).value = "";
        document.getElementById(inputQuantityId).value = "";
        return;
    }

    if (inputValue1 > 45) {
        Swal.fire({
            icon: 'error',
            title: 'Giá trị không hợp lệ!',
            text: 'Vui lòng nhập size dưới 45.'
        });
        document.getElementById(inputSizeId).value = "";
        return;
    }
    if (inputValue1 < 35) {
        Swal.fire({
            icon: 'error',
            title: 'Giá trị không hợp lệ!',
            text: 'Vui lòng nhập size trên 35.'
        });
        document.getElementById(inputSizeId).value = "";
        return;
    }

    // Tiếp tục thực hiện cập nhật hoặc xử lý khác tại đây
}

function ShowCount() {
    $.ajax({
        url: '/shoppingcart/ShowCount',
        type: 'GET',
        success: function (rs) {
            $('#checkout_items').html(rs.Count);
        }
    });
}
function DeleteAll() {
    $.ajax({
        url: '/shoppingcart/DeleteAll',
        type: 'POST',
        success: function (rs) {
            if (rs.Success) {
                LoadCart();
            }
        }
    });
}
function Update(id,quantity,size) {
    $.ajax({
        url: '/shoppingcart/Update',
        type: 'POST',
        data: { id: id, quantity: quantity, size: size },
        success: function (rs) {
            if (rs.Success) {
                LoadCart();
            }
        }
    });
}

function LoadCart() {
    $.ajax({
        url: '/shoppingcart/Partial_Item_Cart',
        type: 'GET',
        success: function (rs) {
            $('#load_data').html(rs);
        }
    });
}

