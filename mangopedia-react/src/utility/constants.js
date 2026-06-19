export const ROUTES = {
  HOME: "/",
  LOGIN: "/login",
  REGISTER: "/register",
  CART: "/cart",
  CHECKOUT: "/checkout",
  MENU_DETAIL: "/menu/:id",
  ORDER_CONFIRMATION: "/order-confirmation",
  MENU_MANAGEMENT: "/menu-management",
  ORDER_MANAGEMENT: "/order-management",
};

export const API_BASE_URL = "https://localhost:7000";

export const CATEGORY = ["Fruit", "Vegetable"];

export const ROLES = {
  ADMIN: "Admin",
  CUSTOMER: "Customer",
  // USER : "User",
};

export const SPECIAL_TAG = [
  "Best Seller",
  "Fresh Arrival",
  "Organic",
  "Seasonal",
];


// if i change my token here then i have to change anywhere else also ?
export const STORAGE_KEYS = {
  TOKEN: "token-mango",
  USER: "user-mango",
};

export const ORDER_STATUS = {
  PENDING: "Pending",
  CONFIRMED: "Confirmed",
  READY_FOR_PICKUP: "ReadyForPickup",
  COMPLETED: "Completed",
  CANCELLED: "Cancelled",
};

export const ORDER_STATUS_OPTIONS = [
  {
    value: ORDER_STATUS.PENDING,
    label: "Pending",
    color: "secondary"
  },
  {
    value: ORDER_STATUS.CONFIRMED,
    label: ORDER_STATUS.CONFIRMED,
    color: "warning",
  },
  {
    value: ORDER_STATUS.READY_FOR_PICKUP,
    label: ORDER_STATUS.READY_FOR_PICKUP,
    color: "info",
  },
  {
    value: ORDER_STATUS.COMPLETED,
    label: ORDER_STATUS.COMPLETED,
    color: "success",
  },
  {
    value: ORDER_STATUS.CANCELLED,
    label: ORDER_STATUS.CANCELLED,
    color: "danger",
  },
];
