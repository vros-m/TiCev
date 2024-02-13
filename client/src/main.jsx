import React from 'react'
import ReactDOM from 'react-dom/client'
import './index.css'
import {
  createBrowserRouter,
  RouterProvider,
} from "react-router-dom";
import Navbar, { NavbarLoader } from './Pages/Navbar';
import Login from './Pages/Login/Login';
import Signup from './Pages/Login/Signup';
import Home, { HomeLoader } from './Pages/Home';
import VideoPlayer from './Pages/VideoPlayer';
import { VideoLoader } from './Pages/VideoPlayer';
import ProfilePage from './Pages/ProfilePage';
import { ProfileLoader } from './Pages/ProfilePage';
import App from './App';
import VideoUploadForm from './Pages/VideoUploadForm';
import CustomizeProfile from './Pages/CustomizeProfile';

const router = createBrowserRouter([
  {
    path: '/',
    element: <Navbar />,
    loader:NavbarLoader,
    children: [
      { path: '/:query?', element: <Home />, loader: HomeLoader },
      { path: 'player/:videoId/:playlistId?', element: <VideoPlayer />, loader: VideoLoader },
      { path: 'profile/:userId?', element: <ProfilePage />, loader: ProfileLoader },
      { path: 'postVideo', element: <VideoUploadForm /> },
      {path:'customizeProfile',element:<CustomizeProfile/>}
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
