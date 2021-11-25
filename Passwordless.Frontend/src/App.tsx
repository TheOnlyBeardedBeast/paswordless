import React from "react";
import Axios from "axios";
import "./App.css";

interface IAuthContext {
  loading: boolean;
  error: boolean;
  inProgress: boolean;
  logged: boolean | null;
  tryAgain: () => void;
  login: (email: string) => Promise<void>;
}

const AuthContext = React.createContext<IAuthContext | null>(null);

export const useAuth = () => {
  const context = React.useContext(AuthContext);

  if (!context) {
    throw new Error("Auth context not initialized");
  }

  return context;
};

export const AuthProvider: React.FC = ({ children }) => {
  const [loading, setLoading] = React.useState<boolean>(false);
  const [error, setError] = React.useState<boolean>(false);
  const [inProgress, setInProgress] = React.useState<boolean>(false);
  const [logged, setLogged] = React.useState<boolean | null>(null);
  const tryCount = React.useRef<number>(0);

  const tryAgain = () => {
    setLoading(false);
    setError(false);
    setLogged(null);
    setInProgress(false);
    tryCount.current = 0;
  };

  const login = async (email: string) => {
    try {
      setError(false);
      setLoading(true);
      const response = await Axios.post(
        "https://localhost:5001/login",
        { Email: email },
        {}
      );

      setInProgress(true);

      const intervalId = window.setInterval(async () => {
        const result = await Axios.post("https://localhost:5001/check", {
          RequestCode: response.data,
        });

        if (result.data === "logged") {
          setLogged(true);
          window.clearInterval(intervalId);
        }

        tryCount.current++;

        if (tryCount.current === 12) {
          setLogged(false);
          window.clearInterval(intervalId);
        }
      }, 5000);
    } catch (error) {
      setError(true);
      setLoading(false);
    } finally {
      setLoading(false);
    }
  };

  return (
    <AuthContext.Provider
      value={{ loading, error, login, inProgress, logged, tryAgain }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const LoginForm: React.FC = ({ children }) => {
  const { login, error, loading, inProgress, logged, tryAgain } = useAuth();
  const inputRef = React.useRef<HTMLInputElement>(null);

  const onSubmit = (e: any) => {
    e.preventDefault();

    if (!inputRef.current?.value) {
      return;
    }

    login(inputRef.current?.value);
  };

  if (logged) {
    return <p>Logged in!</p>;
  }

  if (logged === false) {
    return (
      <div>
        <p>Login failed.</p>
        <button onClick={tryAgain}>Try again</button>
      </div>
    );
  }

  if (loading || inProgress) {
    return <p>Go to your email and confirm your login on any device.</p>;
  }

  return (
    <>
      <form className="loginform" onSubmit={onSubmit}>
        <label>Email address</label>
        <input ref={inputRef} type="text" />
        <button type="submit">Log me in</button>
      </form>
      {error && <p>Shit happens</p>}
    </>
  );
};

function App() {
  return (
    <div className="app">
      <AuthProvider>
        <LoginForm />
      </AuthProvider>
    </div>
  );
}

export default App;
