interface ILogin {
  message: string;
  token: string;
  email: string;
  name: string;
}

interface ILoginCredentials {
    email: string;
    password: string;
}

export type {ILogin as default ,ILoginCredentials}