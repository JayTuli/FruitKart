import { baseApi } from "./baseApi";

export const chatBotApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({
    sendMessage: builder.mutation({
      query: (message) => ({
        url: "/chatbot/send",
        method: "POST",
        body: { message },
      }),
    }),
  }),
});

export const { useSendMessageMutation } = chatBotApi;