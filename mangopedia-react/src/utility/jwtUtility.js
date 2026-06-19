import { jwtDecode } from "jwt-decode";

const decodeJWT = (token) => {
  try {
    return jwtDecode(token);
  } catch {
    return null;
  }
};

export const isTokenExpired = (token) => {
  const decoded = decodeJWT(token);
  return !decoded?.exp || decoded.exp * 1000 < Date.now();
};

export const getUserInfoFromToken = (token) => {
  const decoded = decodeJWT(token);
  if (!decoded) return null;

  return {
    id:    decoded["UserId"]      || "",  // ← "6" from tbl_User
    name:  decoded["unique_name"] || "",  // ← "Admin_1" — this was the bug!
    email: decoded["email"]       || "",
    role:  decoded["role"]        || "",  // ← "Admin" or "Customer"
    exp:   decoded.exp,
  };
};