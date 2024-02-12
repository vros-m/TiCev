import { Link, Outlet, useNavigate } from "react-router-dom";
import Button from '@mui/material/Button';
import { Dialog,DialogContent,IconButton,Avatar,Typography, Backdrop, CardContent, CardActionArea } from "@mui/material";
import LogoutIcon from '@mui/icons-material/Logout';
import AccountBoxIcon from '@mui/icons-material/AccountBox';
import NotificationsNoneIcon from '@mui/icons-material/NotificationsNone';
import NotificationsActiveIcon from '@mui/icons-material/NotificationsActive';
import CloseIcon from '@mui/icons-material/Close';
import SearchBar from "../GeneralComponents/Searchbar";
import { useContext, useEffect, useRef, useState } from "react";
import UserContext from "./Contexts/UserContext";


export default function Navbar()
{
  const [dialogOpen, setDialogOpen] = useState(false)
  const [userState,setUserState] = useContext(UserContext)
  


  function logout()
  {
    setUserState(null)
  }
  const notificationsRef = useRef(null)
  const navigate = useNavigate()
  useEffect(() =>
  {
    if (userState == null) {
      navigate('/login')
    }

  }, [userState])

  return <div id="root" >
          <div className="d-flex flex-row justify-content-between" style={{
      height: "100px",
      backgroundColor: "white",
          color: "black",
          position: 'fixed',
          top: '0px',
          padding: '10px',
          paddingLeft: '30px',
          zIndex: '1000',
          paddingRight: '20px',
              width:'100%'
      }}>
          <Link style={{ color: 'black', fontSize: "2.5rem" }} to="/">TiCev</Link>
          <SearchBar onSearch={() =>{}} width={'500px'}/>
          <div className="d-flex flex-row align-items-center">
              <Button ref={notificationsRef}
                  sx={{ color: 'black' }}
                  onClick={(ev)=>setDialogOpen((oldValue=>!oldValue))}
              ><NotificationsNoneIcon sx={{ width: '35px', height: '35px' }} /></Button>
              <NotificationDialog isOpen={dialogOpen} onClose={ev =>setDialogOpen(false)} anchorEl={notificationsRef}/>
        <Button sx={{ color: 'black' }}><AccountBoxIcon sx={{ width: '35px', height: '35px' }}
          onClick={ev => navigate('profile')} /></Button>
              <Button sx={{color:'black'}} onClick={ev=>logout()}><LogoutIcon sx={{width:'35px',height:'35px'} }/></Button>
          </div>
      </div>
      <Outlet/>
  </div>

}

const NotificationCard = ({ notification }) => {
    const { userAvatar, content, timestamp, videoThumbnail } = notification;
  
    return (
      <div
        style={{
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
          padding: '8px',
          borderBottom: '1px solid #e0e0e0',
          minHeight: '80px',
                width: '100%',
          cursor:'pointer'
        }}
        ><CardActionArea sx={{ display: 'flex', alignItems: 'center' }}>
                     <div style={{ display: 'flex', alignItems: 'center' }}>
          {/* User Avatar */}
          <Avatar src={userAvatar} alt="User Avatar" style={{ marginRight: '12px', width: '60px', height: '60px' }} />
          {/* Content */}
          <div>
            <Typography variant="body1">{content}</Typography>
            <Typography variant="caption">{timestamp}</Typography>
          </div>
        </div>
        {/* Video Thumbnail */}
        <img src={videoThumbnail} alt="Video Thumbnail" style={{ height: '60px', width: 'auto' }} />   
      </CardActionArea>

      </div>
    );
  };
  
const NotificationDialog = ({ isOpen, onClose, anchorEl}) => {
    const [notifications, setNotifications] = useState([])
    useEffect(() => {
        const rawData = new Array(20).fill({
            userAvatar: "https://via.placeholder.com/30x30",
            content: "TESTING CONTENT TESTING NEW VIDEO OR NEW COMMENT OR NEW REPLY",
            timestamp:"2 minutes ago", videoThumbnail :'https://via.placeholder.com/345x200'
        })
        setNotifications(rawData)
    },[])
    return (
        <Dialog
            open={isOpen}
            onClose={onClose}
            maxWidth="md"
            fullWidth
            PaperProps={{
                style: {
                    position: 'absolute',
                    width: '600px',
                    top: anchorEl.current ? anchorEl.current.getBoundingClientRect().bottom : 0,
                    right: '50px',
                },
            }}
      >
        <DialogContent>
       
          {/* Notification Cards */}
          <div style={{ maxHeight: '400px', overflowY: 'auto' }}>
            {notifications.map((notification, index) => (
              <NotificationCard key={index} notification={notification} />
            ))}
          </div>
        </DialogContent>
      </Dialog>
    );
  };
  

