import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import './header.css';
import './navbar.css';

const Header = () => {
  const [activeItem, setActiveItem] = useState("Forms");
  const navItems = ["Forms", "+Proposta", "Validar", "Dashbord", "Clientes"];

  return (
    <>
      <header className="header">
        <div className="header-content">
          <div className="logo-titulo">
            <img
              src="./src/img/logo-eficaz.png"
              alt="Logo"
              className="header-img"
            />
            <p className="header-title">
              Gerenciador de Propostas <br />
              CRM
            </p>
          </div>
          <div className="icon">
            <img
              className="user-icon"
              src="./src/img/user-icon.svg"
              alt="Usuário"
            />
          </div>
        </div>
      </header>
      <nav className="navbar-container">
        {navItems.map((item) => (
      
          <Link
            key={item}
            to={`/${item.toLowerCase().replace("+", "")}`} 
            className={`nav-button ${item === activeItem ? "active" : ""}`}
            onClick={() => setActiveItem(item)}
          >
            {item}
          </Link>
        ))}
      </nav>
    </>
  );
};

export default Header;
