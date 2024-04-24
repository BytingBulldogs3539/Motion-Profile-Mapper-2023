import React, { useState } from 'react';
import logo from './logo.svg';
import './App.css';

import { Icon, Menu, MenuItem, Tab } from 'semantic-ui-react';
import MotionProfiler from './Pages/MotionProfiler';
import Home from './Pages/Home';

export default function App() {
  const [darkMode, setDarkMode] = useState(true);
  const [menuSelection, setMenuSelection] = useState('home');

  return (
    <div id='main'>
      <Menu icon vertical secondary pointing>
        <MenuItem
          name='home'
          active={menuSelection === 'home'}
          onClick={() => setMenuSelection('home')}
        >
          <Icon name='home' />
        </MenuItem>

        <MenuItem
          name='motion profiler'
          active={menuSelection === 'motion profiler'}
          onClick={() => setMenuSelection('motion profiler')}
        >
          <Icon name='map' />
        </MenuItem>

        <MenuItem
          name='constants'
          active={menuSelection === 'constants'}
          onClick={() => setMenuSelection('constants')}
        >
          <Icon name='key' />
        </MenuItem>
      </Menu>
      {menuSelection === 'home' && <Home darkMode={darkMode} />}
      {menuSelection === 'motion profiler' && <MotionProfiler darkMode={darkMode} />}
    </div>
  );
}