
import React from "react";
import { Outlet } from "react-router-dom"; // <--- ESSENCIAL
import Header from "./header.jsx";
// ...

const Layout = () => {
  return (
    <div>
      <Header />

      <div className="main-content">
        <div className="page-content">
          {}
          <Outlet />
        </div>
      </div>
    </div>
  );
};

export default Layout;
