import { baseApi } from "./baseApi";

export const authApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({
    //create all endpoints

    loginUser: builder.mutation({
      query: (formData) => ({
        url: "/account/login",
        method: "POST",
        body: formData,
      }),
    }),

    registerUser: builder.mutation({
      query: (formData) => ({
        url: "/account/register",
        method: "POST",
        body: formData,
      }),
    }),
  }),
});

export const { useLoginUserMutation, useRegisterUserMutation,  } = authApi;
