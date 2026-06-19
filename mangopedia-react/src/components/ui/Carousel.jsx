function Carousel() {
  return (
    <>
      {/* Add floating animation styles */}
      <style>
        {`
          @keyframes float {
            0%, 100% { transform: translateY(0px); }
            50% { transform: translateY(-20px); }
          }
          @keyframes pulse {
            0%, 100% { opacity: 0.4; }
            50% { opacity: 0.8; }
          }
        `}
      </style>

      <div
        id="heroCarousel"
        className="carousel slide mb-4"
        data-bs-ride="carousel"
        data-bs-interval="6000"
      >
        <div className="carousel-indicators">
          <button
            type="button"
            data-bs-target="#heroCarousel"
            data-bs-slide-to="0"
            className="active"
            aria-current="true"
            aria-label="Slide 1"
          ></button>
          <button
            type="button"
            data-bs-target="#heroCarousel"
            data-bs-slide-to="1"
            aria-label="Slide 2"
          ></button>
        </div>

        <div className="carousel-inner">
          {/* First Slide - Fresh Food Theme */}
          <div className="carousel-item active">
            <div
              className="py-5 d-flex align-items-center"
              style={{
                minHeight: "600px",
                height: "600px",
                background:
                  "#006e37",
                position: "relative",
              }}
            >
              {/* Animated floating elements */}
              <div
                style={{
                  position: "absolute",
                  top: "10%",
                  right: "10%",
                  width: "100px",
                  height: "100px",
                  background: "rgba(255, 193, 7, 0.1)",
                  borderRadius: "50%",
                  filter: "blur(20px)",
                  animation: "float 6s ease-in-out infinite",
                }}
              ></div>
              <div
                style={{
                  position: "absolute",
                  bottom: "20%",
                  left: "15%",
                  width: "60px",
                  height: "60px",
                  background: "rgba(40, 167, 69, 0.1)",
                  borderRadius: "50%",
                  filter: "blur(20px)",
                  animation: "float 4s ease-in-out infinite reverse",
                }}
              ></div>

              {/* Subtle overlay */}
              <div
                style={{
                  position: "absolute",
                  top: 0,
                  left: 0,
                  right: 0,
                  bottom: 0,
                  background: "rgba(0,0,0,0.1)",
                  pointerEvents: "none",
                }}
              ></div>

              <div className="container h-100">
                <div className="row align-items-center w-100 h-100">
                  <div className="col-lg-7">
                    <div className="mb-4">
                      <span className="badge bg-gradient bg-warning fs-6 px-3 py-2 rounded-pill">
                        <i className="bi bi-basket me-2"></i>Fresh & Tasty
                      </span>
                    </div>
                    <h1 className="display-2 fw-bold mb-4 text-white">
                      Savor Every
                      <span className="d-block text-warning fst-italic">
                        Bite
                      </span>
                    </h1>
                    <p className="lead mb-4 text-white-50 fs-4">
                      Experience the Farm Fresh - Hand plucked fruits daily.
                      <span className="text-warning fw-semibold">
                        Plucked fresh daily
                      </span>
                      , packed with Love.
                    </p>

                    <div className="row mb-4">
                      <div className="col-md-6 mb-2">
                        <div className="d-flex align-items-center">
                          <i className="bi bi-check-circle-fill text-success me-2"></i>
                          <small className="text-white-50">
                            Fresh Fruits Daily
                          </small>
                        </div>
                      </div>
                      <div className="col-md-6 mb-2">
                        <div className="d-flex align-items-center">
                          <i className="bi bi-check-circle-fill text-success me-2"></i>
                          <small className="text-white-50">
                            Quick Pickup Service
                          </small>
                        </div>
                      </div>
                    </div>

                    <div className="d-flex gap-3 flex-wrap">
                      <a
                        href="#menu"
                        className="btn btn-warning btn-lg rounded-pill px-5 py-3 fw-bold shadow-lg"
                        style={{
                          background:
                            " #44ff00",
                          border: "none",
                          transition: "all 0.3s ease",
                        }}
                      >
                        <i className="bi bi-basket me-2"></i>Place your Order Now
                      </a>
                    </div>
                  </div>

                  <div className="col-lg-5 d-none d-lg-block text-center">
                    <div className="position-relative">
                      {/* Main feature card */}
                      <div
                        className="card bg-gradient shadow-lg border-0"
                        style={{
                          background: "rgba(255, 255, 255, 0.1)",
                          backdropFilter: "blur(10px)",
                          border: "1px solid rgba(255, 255, 255, 0.2)",
                        }}
                      >
                        <div className="card-body p-5 text-center">
                          <div className="mb-4">
                            <i className="bi bi-cup-hot display-1 text-warning"></i>
                          </div>
                          <h3 className="text-white mb-3">Fresh & Delicious</h3>
                          
                          <div className="row text-center">
                            <div className="col-6">
                              <div className="border-end border-light border-opacity-25">
                                <h4 className="text-warning mb-0">50+</h4>
                                <small className="text-white-50">Fruits/Vegetables</small>
                              </div>
                            </div>
                            <div className="col-6">
                              <h4 className="text-warning mb-0">15min</h4>
                              <small className="text-white-50">
                                Pickup Time
                              </small>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          {/* Second Slide*/}
          <div className="carousel-item">
            <div
              className="py-5 d-flex align-items-center"
              style={{
                minHeight: "600px",
                height: "600px",
                background:
                  "linear-gradient(135deg, #28a745 0%, #20c997 50%, #17a2b8 100%)",
                position: "relative",
              }}
            >
              {/* Animated floating elements */}
              <div
                style={{
                  position: "absolute",
                  top: "15%",
                  left: "10%",
                  width: "80px",
                  height: "80px",
                  background: "rgba(255, 193, 7, 0.15)",
                  borderRadius: "50%",
                  filter: "blur(15px)",
                  animation: "pulse 4s ease-in-out infinite",
                }}
              ></div>
              <div
                style={{
                  position: "absolute",
                  bottom: "10%",
                  right: "20%",
                  width: "120px",
                  height: "120px",
                  background: "rgba(255, 255, 255, 0.1)",
                  borderRadius: "50%",
                  filter: "blur(25px)",
                  animation: "float 5s ease-in-out infinite",
                }}
              ></div>

              {/* Overlay for better text readability */}
              <div
                style={{
                  position: "absolute",
                  top: 0,
                  left: 0,
                  right: 0,
                  bottom: 0,
                  background: "rgba(0,0,0,0.2)",
                  pointerEvents: "none",
                }}
              ></div>

              <div className="container h-100">
                <div className="row align-items-center w-100 h-100">
                  <div className="col-lg-7">
                    <div className="mb-4">
                      <span className="badge bg-success text-white fs-6 px-3 py-2 rounded-pill">
                        <i className="bi bi-shield-check me-2"></i>
                        Top Notch Quality
                      </span>
                    </div>
                    <h1 className="display-2 fw-bold mb-4 text-white">
                      Premium
                      <span className="d-block text-warning fst-italic">
                        Quality
                      </span>
                    </h1>
                    <p className="lead mb-4 text-white fs-4">
                      Top Notch Quality - Handpicked fruits for the freshest taste.
                      <span className="text-warning fw-semibold">
                        Taste the difference
                      </span>{" "}
                      in every bite.
                    </p>

                    {/* Feature highlights */}
                    <div className="row mb-4">
                      <div className="col-md-6 mb-2">
                        <div className="d-flex align-items-center">
                          <i className="bi bi-award-fill text-warning me-2"></i>
                          <small className="text-white-50">
                            Farm Fresh Quality
                          </small>
                        </div>
                      </div>
                      <div className="col-md-6 mb-2">
                        <div className="d-flex align-items-center">
                          <i className="bi bi-star-fill text-warning me-2"></i>
                          <small className="text-white-50">
                            5-Star Customer Rating
                          </small>
                        </div>
                      </div>
                    </div>

                    <div className="d-flex gap-3 flex-wrap">
                      <a
                        href="#menu"
                        className="btn btn-success btn-lg rounded-pill px-5 py-3 fw-bold shadow-lg"
                        style={{
                          background:
                            "linear-gradient(45deg, #28a745, #20c997)",
                          border: "none",
                          transition: "all 0.3s ease",
                        }}
                      >
                        <i className="bi bi-star me-2"></i>View Cart
                      </a>
                    </div>
                  </div>

                  <div className="col-lg-5 d-none d-lg-block text-center">
                    <div className="position-relative">
                      {/* Main feature card */}
                      <div
                        className="card bg-gradient shadow-lg border-0"
                        style={{
                          background: "rgba(255, 255, 255, 0.1)",
                          backdropFilter: "blur(10px)",
                          border: "1px solid rgba(255, 255, 255, 0.2)",
                        }}
                      >
                        <div className="card-body p-5 text-center">
                          <div className="mb-4">
                            <i className="bi bi-award display-1 text-warning"></i>
                          </div>
                          <h3 className="text-white mb-3">Customers First Choice</h3>
                          <p className="text-white-50 mb-4">
                            Trusted for consistent freshness and superior quality
                          </p>
                          <div className="row text-center">
                            <div className="col-6">
                              <div className="border-end border-light border-opacity-25">
                                <h4 className="text-warning mb-0">98%</h4>
                                <small className="text-white-50">
                                  Happy Customers
                                </small>
                              </div>
                            </div>
                            <div className="col-6">
                              <h4 className="text-warning mb-0">4.9★</h4>
                              <small className="text-white-50">
                                Average Rating
                              </small>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
          </div>

        <button
          className="carousel-control-prev"
          type="button"
          data-bs-target="#heroCarousel"
          data-bs-slide="prev"
        >
          <span
            className="carousel-control-prev-icon"
            aria-hidden="true"
          ></span>
          <span className="visually-hidden">Previous</span>
        </button>
        <button
          className="carousel-control-next"
          type="button"
          data-bs-target="#heroCarousel"
          data-bs-slide="next"
        >
          <span
            className="carousel-control-next-icon"
            aria-hidden="true"
          ></span>
          <span className="visually-hidden">Next</span>
        </button>
      </div>
    </>
  );
}

export default Carousel;
