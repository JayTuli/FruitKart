import { baseApi } from "./baseApi";

export const menuItemsApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({

    //creating all enpoints 
    getMenuItems: builder.query({
      query: () => "/menuItem",//M ->m
      providesTags: ["MenuItem"],
      transformResponse: (response) => {
        if (response && response.result && Array.isArray(response.result)) {
          return response.result;
        }
        if (response && Array.isArray(response)) {
          return response;
        }
        return [];
      },
    }),

    getMenuItemById: builder.query({
      query: (id) => `/menuitem/${id}`,
      providesTags: (result, error,  id ) => [{ type: "MenuItem", id }],
      transformResponse: (response) => {
        if (response && response.result) 
          return response.result;
        
        return response;
      },
    }),

    createMenuItem: builder.mutation({
      query: (formData) => ({
        url: "/menuitem",
        method: "POST",
        body: formData,
      }),
      invalidatesTags: ["MenuItem"],
    }),

    deleteMenuItem: builder.mutation({
      query: (id) => ({
        url: `/menuitem/${id}`,
        method: "DELETE",
      }),
      invalidatesTags: ["MenuItem"],
    }),

    updateMenuItem: builder.mutation({
      query: ({ id, formData }) => ({
        url: `/menuitem/${id}`,
        method: "PUT",
        body: formData,
      }),
      invalidatesTags: (result, error, { id }) => [{ type: "MenuItem", id }],
    }),
  }),
});

export const {
  useGetMenuItemsQuery,
  useCreateMenuItemMutation,
  useDeleteMenuItemMutation,
  useUpdateMenuItemMutation,
  useGetMenuItemByIdQuery,
} = menuItemsApi;
