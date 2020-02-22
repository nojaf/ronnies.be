import React, { useState } from "react";
import PropTypes from 'prop-types'
import { redirectUri, useAuth0 } from "../Auth";
import { A, usePath } from "hookrouter";
import {
  Navbar,
  NavItem,
  NavLink,
  NavbarBrand,
  NavbarToggler
} from "reactstrap";
import Collapse from "reactstrap/es/Collapse";
import Nav from "reactstrap/es/Nav";
import { WhenEditor } from '../bin/Components';

const Navigation = ({ role }) => {
  const [collapsed, setCollapsed] = useState(true);
  const { loginWithRedirect, logout, isAuthenticated, user } = useAuth0();

  const path = usePath();
  const loginHandler = ev => {
    ev.preventDefault();
    loginWithRedirect({ redirect_uri: redirectUri });
  };
  const logoutHandler = ev => {
    ev.preventDefault();
    logout();
  };
  let loginButton = isAuthenticated ? (
    <NavItem onClick={logoutHandler}>
      <NavLink href={"#"}>Uitloggen</NavLink>
    </NavItem>
  ) : (
    <NavItem onClick={loginHandler}>
      <NavLink href={"#"}>Inloggen</NavLink>
    </NavItem>
  );

  const userElement = user && (
    <span className={"navbar-text d-flex align-items-center"}>
      <img src={user.picture} className={"avatar"} alt={"user avatar"} />
      {user.nickname}
    </span>
  );

  const MenuLink = ({ link, name }) => {
    const isActive = link === path;
    return (
      <NavItem key={link}>
        <NavLink href={link} tag={A} active={isActive}>
          {name}
        </NavLink>
      </NavItem>
    );
  };

  // const extraMenuItems = (() => {
  //   if (!isAuthenticated) {
  //     return [navLink("/", "Koarte")];
  //   } else {
  //     switch (role) {
  //       case "Admin":
  //       case "Editor":
  //         return [
  //           navLink("/", "Koarte"),
  //           navLink("/add-location", "E nieuwen toevoegen"),
  //           navLink("/user-scores", "Klassement"),
  //           navLink("/rules", "Reglement"),
  //           navLink("/settings", "Instellingen")
  //         ];
  //       default:
  //         return [navLink("/", "Koarte"), navLink("/settings", "Instellingen")];
  //     }
  //   }
  // })();

  return (
    <Navbar color="primary" dark expand={"md"}>
      <NavbarBrand href={"/"}>
        <img src={"assets/r-white.png"} alt={"Logo ronnies.be"} />
      </NavbarBrand>
      <NavbarToggler onClick={() => setCollapsed(!collapsed)} />
      <Collapse isOpen={!collapsed} navbar>
        <Nav navbar className="mr-auto">
          <MenuLink name={'Koarte'} link={'/'} />
          <WhenEditor>
            {/*<MenuLink name={'E nieuwen toevoegen'} link={'/add-location'} />*/}
            {/*<MenuLink name={'Klassement'} link={'/user-scores'} />*/}
            {/*<MenuLink name={'Reglement'} link={'/rules'} />*/}
            <MenuLink name={'Instellingen'} link={'/settings'} />
          </WhenEditor>
          {loginButton}
        </Nav>
        {userElement}
      </Collapse>
    </Navbar>
  );
};

Navigation.propTypes = {
  role: PropTypes.string.isRequired
};

export default Navigation;
