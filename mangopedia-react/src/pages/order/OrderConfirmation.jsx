import { useLocation, useNavigate } from "react-router-dom";
import { ROUTES } from "../../utility/constants";

function OrderConfirmation() {
  const location = useLocation();
  const navigate = useNavigate();
  const orderData = location.state?.orderData;

  // ✅ If navigated here without order data, redirect home
  if (!orderData) {
    return (
      <div className="container py-5 text-center">
        <h4 className="text-muted">No order data found.</h4>
        <button className="btn btn-primary mt-3" onClick={() => navigate(ROUTES.HOME)}>
          Go Home
        </button>
      </div>
    );
  }

  return (
    <div className="container py-5">
      <div className="row justify-content-center">
        <div className="col-md-6">

          {/* Success Header */}
          <div className="text-center mb-4">
            <div className="mb-3">
              <i
                className="bi bi-check-circle-fill text-success"
                style={{ fontSize: "3rem" }}
              ></i>
            </div>
            <h2 className="fw-bold text-success mb-2">Order Confirmed!</h2>
            <p className="text-muted">Thank you for your order.</p>
          </div>

          {/* Order Details Card */}
          <div className="card border shadow-sm">
            <div className="card-body p-4">
              <h5 className="fw-bold mb-4 text-center">Order Details</h5>
              <hr />

              <div className="mb-3 d-flex justify-content-between">
                <span className="text-muted">Order ID</span>
                {/* ✅ FIX: orderNumber comes from result.Result?.orderHeaderId set in Cart.jsx */}
                <strong>#{orderData.orderNumber ?? "N/A"}</strong>
              </div>

              <div className="mb-3 d-flex justify-content-between">
                <span className="text-muted">Pickup Name</span>
                <strong>{orderData.pickUpName}</strong>
              </div>

              <div className="mb-3 d-flex justify-content-between">
                <span className="text-muted">Email</span>
                <strong>{orderData.pickUpEmail}</strong>
              </div>

              <div className="mb-3 d-flex justify-content-between">
                <span className="text-muted">Phone Number</span>
                <strong>{orderData.pickUpPhoneNumber}</strong>
              </div>

              <div className="mb-3 d-flex justify-content-between">
                <span className="text-muted">Number of Items</span>
                <strong>{orderData.totalItems}</strong>
              </div>

              <div className="mb-4 d-flex justify-content-between">
                <span className="text-muted">Order Total</span>
                {/* ✅ FIX: orderTotal comes from result.Result?.orderTotal set in Cart.jsx */}
                <strong className="text-primary fs-5">
                  ₹{orderData.orderTotal != null ? Number(orderData.orderTotal).toFixed(2) : "0.00"}
                </strong>
              </div>

              <hr />

              <div className="alert alert-info small mb-4">
                <i className="bi bi-clock me-2"></i>
                <strong>Ready in 15-20 mins</strong> after order confirmation
              </div>

              <div className="text-center">
                <button
                  className="btn btn-primary px-5"
                  onClick={() => navigate(ROUTES.HOME)}
                >
                  <i className="bi bi-bag me-2"></i>Continue Shopping
                </button>
              </div>
            </div>
          </div>

        </div>
      </div>
    </div>
  );
}

export default OrderConfirmation;
