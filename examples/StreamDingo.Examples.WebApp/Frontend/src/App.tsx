import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import Layout from './components/Layout'
import UsersPage from './pages/UsersPage'
import BusinessesPage from './pages/BusinessesPage'
import RelationshipsPage from './pages/RelationshipsPage'
import HomePage from './pages/HomePage'

function App() {
  return (
    <Router>
      <Layout>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/users" element={<UsersPage />} />
          <Route path="/businesses" element={<BusinessesPage />} />
          <Route path="/relationships" element={<RelationshipsPage />} />
        </Routes>
      </Layout>
    </Router>
  )
}

export default App
