import AppRoutes from "./routes/AppRouter";
import Header from "./components/layout/Header";
import Footer from "./components/layout/Footer";
import { useSelector, useDispatch } from "react-redux";
import { useEffect } from "react";
import { ToastContainer } from "react-toastify";
import ChatBot from "./components/chatBot/ChatBot";
import { loadCart } from "./store/slice/cartSlice";

function App() {
  const dispatch = useDispatch();
  const theme = useSelector((state) => state.theme.theme);
  const { user } = useSelector((state) => state.auth);  

  useEffect(() => {
    if (user?.id) {
      dispatch(loadCart(user.id));
    }
  }, [user?.id, dispatch]);

  useEffect(() => {
    document.body.setAttribute("data-bs-theme", theme);
  }, [theme]);

  const getThemeStyles = () => {
    if (theme === "dark") {
      return {
        background: `linear-gradient(135deg, #686464 0%, #000000 25%, #2d1b69 50%, #11998e 75%, #38ef7d 100%)`,
      };
    }
    return {
      background: `linear-gradient(135deg, #a8edea 0%, #fbcd91 25%, #d299c2 50%, #fef9d7 75%, #a8edea 100%)`,
    };
  };

  return (
    <div className="d-flex flex-column min-vh-100 bg-body" style={getThemeStyles()}>
      <Header />
      <main className="flex-grow-1">
        <AppRoutes />
      </main>
      <Footer />
      <ToastContainer
        position="top-right"
        autoClose={5000}
        hideProgressBar={false}
        newestOnTop={false}
        closeOnClick={false}
        rtl={false}
        pauseOnFocusLoss
        draggable
        pauseOnHover
        theme="light"
      />
      <ChatBot />
    </div>
  );
}

export default App;