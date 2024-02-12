import React from 'react'
import ReactDOM from 'react-dom/client'
import './index.css'
import {
  createBrowserRouter,
  RouterProvider,
} from "react-router-dom";
import Navbar from './Pages/Navbar';
import Login from './Pages/Login/Login';
import Signup from './Pages/Login/Signup';
import Home, { HomeLoader } from './Pages/Home';
import VideoPlayer from './Pages/VideoPlayer';
import { VideoLoader } from './Pages/VideoPlayer';
import ProfilePage from './Pages/ProfilePage';
import { ProfileLoader } from './Pages/ProfilePage';
import UserContext from './Pages/Contexts/UserContext';
import App from './App';

const router = createBrowserRouter([
  {
    path: '/',
    element: <Navbar />,
    children: [
      { path: '/', element: <Home />, loader: HomeLoader },
      { path: 'player/:videoId/:playlistId?', element: <VideoPlayer />, loader: VideoLoader },
      {path:'profile/:userId?',element:<ProfilePage/>,loader:ProfileLoader},
    ]
  },
  {
    path: 'login',
    element: <Login />
  },
  {
    path: 'signup',
    element: <Signup />
  }
])
ReactDOM.createRoot(document.getElementById('root')).render(
  <App>
    <RouterProvider router={router}/>
  </App>
)
