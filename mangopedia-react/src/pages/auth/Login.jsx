// import { ROUTES } from "../../utility/constants"; // removed the roles
// import { useState } from "react";
// import { Link, useNavigate, useLocation } from "react-router-dom";
// import { useLoginUserMutation } from "../../store/api/authApi";
// import { toast } from "react-toastify";
// import { setAuth } from "../../store/slice/authSlice";
// import { useDispatch } from "react-redux";
// import { getUserInfoFromToken } from "../../utility/jwtUtility";

// function Login() {
//   const navigate = useNavigate();
//   const location = useLocation();
//   const dispatch = useDispatch();
//   const [formData, setFormData] = useState({
//     email: "",
//     password: "",
//   });

//   const [loginUser, { isLoading }] = useLoginUserMutation(); 

//   const handleChange = (e) => {
//     setFormData({
//       ...formData, // spread operator to copy existing formData
//       [e.target.name]: e.target.value
//     });
//   };
//   const handleSubmit = async (e) => {
//     e.preventDefault();

//     if (!formData.email || !formData.password) {
//       toast.error("All the fields are Required !");
//       return;
//     }

//     try {
//       const result = await loginUser(formData).unwrap();
//       if (result.isSuccess) {
//         const token = result.result.token;
//         const user = getUserInfoFromToken(token);
//         dispatch(setAuth({ user, token }));
//         toast.success('Welcome Back ');
//         // dispatch(setAuth({ user, token }));

//         const from = location.state?.from || ROUTES.HOME;
//         navigate(from, { replace: true });

//       } else {
//         toast.error(result.errorMessages?.[0] || "Login Failed");
//       }
//     } catch (error) {
//       toast.error(error.data?.errorMessages?.[0] || "Invalid credentials ! or Login First");
//     }
//   };

//   return (
//     <div className="min-vh-100 d-flex align-items-center py-5">
//       <div className="container">
//         <div className="row g-5 align-items-center justify-content-center">
//           {/* Marketing / Side Panel (desktop) */}
//           <div className="col-lg-5 d-none d-lg-block">
//             <div className="text-center px-4">
//               <div className="mb-4">
//                 <i
//                   className="bi bi-basket text-primary"
//                   style={{ fontSize: "4rem" }}
//                 ></i>
//               </div>
//               <h2 className="fw-bold mb-3">Welcome to MangoFusion</h2>
//               <p className="text-muted mb-4">
//                 Sign in to explore fresh flavors, manage your cart, and place
//                 your orders seamlessly.
//               </p>
//               <div className="text-start mx-auto" style={{ maxWidth: "360px" }}>
//                 <div className="d-flex mb-2 small">
//                   <i className="bi bi-check-circle-fill text-primary me-2"></i>
//                   <span>Secure account access</span>
//                 </div>
//                 <div className="d-flex mb-2 small">
//                   <i className="bi bi-check-circle-fill text-primary me-2"></i>
//                   <span>Track past orders</span>
//                 </div>
//                 <div className="d-flex mb-2 small">
//                   <i className="bi bi-check-circle-fill text-primary me-2"></i>
//                   <span>Save your favorites</span>
//                 </div>
//               </div>
//             </div>
//           </div>

//           {/* Form Panel */}
//           <div className="col-md-8 col-lg-6 col-xl-5">
//             <div className="border rounded-4 shadow-sm p-4 p-lg-5 bg-body-tertiary ">
//               <div className="mb-4 text-center">
//                 <h3 className="fw-bold mb-2">Sign-In</h3>
//                 <p className="text-muted small mb-1">To Your Acccount</p>
//               </div>

//               <form onSubmit={handleSubmit}>
//                 <div className="form-floating mb-3">
//                   <input
//                     type="email"
//                     className="form-control"
//                     id="email"
//                     name="email"
//                     placeholder="name@example.com"
//                     value={formData.email}
//                     onChange={handleChange}
//                   />
//                   <label htmlFor="email">Email address</label>
//                 </div>

//                 <div className="mb-3">
//                   <div className="input-group">
//                     <div className="form-floating flex-grow-1">
//                       <input
//                         type="password"
//                         className="form-control"
//                         id="password"
//                         name="password"
//                         placeholder="Password"
//                         value={formData.password}
//                         onChange={handleChange}
//                       />
//                       <label htmlFor="password">Password</label>
//                     </div>
//                   </div>
//                 </div>

//                 <button
//                   type="submit"
//                   disabled={isLoading}
//                   className="btn btn-primary w-100 py-2 mb-3"
//                 >
//                   {isLoading ? (
//                     <>
//                       <span
//                         className="spinner-border spinner-border-sm me-2"
//                         role="status"
//                       ></span>
//                       Signing In....{" "}
//                     </>
//                   ) : (
//                     <>Login</>
//                   )}
//                 </button>
//               </form>
//               <div className="text-center small">
//                 <span className="text-muted">No account ? </span>
//                 <Link to={ROUTES.REGISTER} className="fw-semibold"> 
//                   Create Your Account 
//                 </Link>
//               </div>
//               <div className="text-center mt-3 small">
//                 <Link to={ROUTES.HOME} className="text-decoration-none">
//                   <i className="bi bi-arrow-left me-1"></i>Back to Home
//                 </Link>
//               </div>
//             </div>
//           </div>
//         </div>
//       </div>
//     </div>
//   );
// }

// export default Login;
import { ROUTES } from "../../utility/constants";
import { useState } from "react";
import { Link, useNavigate, useLocation } from "react-router-dom";
import { useLoginUserMutation } from "../../store/api/authApi";
import { toast } from "react-toastify";
import { setAuth } from "../../store/slice/authSlice";
import { loadCart } from "../../store/slice/cartSlice";
import { useDispatch } from "react-redux";
import { getUserInfoFromToken } from "../../utility/jwtUtility";

function Login() {
  const navigate = useNavigate();
  const location = useLocation();
  const dispatch = useDispatch();
  const [formData, setFormData] = useState({ email: "", password: "" });
  const [loginUser, { isLoading }] = useLoginUserMutation();

  const handleChange = (e) =>
    setFormData({ ...formData, [e.target.name]: e.target.value });

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!formData.email || !formData.password) {
      toast.error("All fields are required!");
      return;
    }
    try {
      const result = await loginUser({
        Email:    formData.email,
        Password: formData.password,
      }).unwrap();

      if (result.isSuccess) {
        const token = result.result.token;
        const user  = getUserInfoFromToken(token);
        dispatch(setAuth({ user, token }));
        dispatch(loadCart(user.id));  // ← load this user's cart
        toast.success(`Welcome back ${user.name}!`);
        navigate(location.state?.from || ROUTES.HOME, { replace: true });
      } else {
        toast.error(result.errorMessages?.[0] || "Login failed");
      }
    } catch (error) {
      toast.error(error.data?.errorMessages?.[0] || "Invalid credentials!");
    }
  };

  return (
    <div className="min-vh-100 d-flex align-items-center py-5">
      <div className="container">
        <div className="row g-5 align-items-center justify-content-center">
          <div className="col-lg-5 d-none d-lg-block">
            <div className="text-center px-4">
              <i className="bi bi-basket text-primary" style={{ fontSize: "4rem" }}></i>
              <h2 className="fw-bold mb-3 mt-3">Welcome to FruitCart</h2>
              <p className="text-muted mb-4">Sign in to explore fresh flavors and place your orders.</p>
              {["Secure account access", "Track past orders", "Save your favorites"].map((t) => (
                <div className="d-flex mb-2 small" key={t}>
                  <i className="bi bi-check-circle-fill text-primary me-2"></i>
                  <span>{t}</span>
                </div>
              ))}
            </div>
          </div>

          <div className="col-md-8 col-lg-6 col-xl-5">
            <div className="border rounded-4 shadow-sm p-4 p-lg-5 bg-body-tertiary">
              <div className="mb-4 text-center">
                <h3 className="fw-bold mb-2">Sign In</h3>
                <p className="text-muted small">To Your Account</p>
              </div>
              <form onSubmit={handleSubmit}>
                <div className="form-floating mb-3">
                  <input type="email" className="form-control" id="email"
                    name="email" placeholder="name@example.com"
                    value={formData.email} onChange={handleChange} />
                  <label htmlFor="email">Email address</label>
                </div>
                <div className="form-floating mb-3">
                  <input type="password" className="form-control" id="password"
                    name="password" placeholder="Password"
                    value={formData.password} onChange={handleChange} />
                  <label htmlFor="password">Password</label>
                </div>
                <button type="submit" disabled={isLoading}
                  className="btn btn-primary w-100 py-2 mb-3">
                  {isLoading
                    ? <><span className="spinner-border spinner-border-sm me-2"></span>Signing In...</>
                    : "Login"}
                </button>
              </form>
              <div className="text-center small">
                <span className="text-muted">No account? </span>
                <Link to={ROUTES.REGISTER} className="fw-semibold">Create Your Account</Link>
              </div>
              <div className="text-center mt-3 small">
                <Link to={ROUTES.HOME} className="text-decoration-none">
                  <i className="bi bi-arrow-left me-1"></i>Back to Home
                </Link>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Login;